using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace Planner.Services;

public class AppSettings
{
    public ElementTheme Theme { get; set; } = ElementTheme.Light;
    public bool EnableMica { get; set; } = true;
    public string ProfileName { get; set; } = "Planner Space";
    public string ProfilePlan { get; set; } = "Personal Plan";
    public string AvatarPath { get; set; } = "ms-appx:///Assets/profile_avatar.png";
}

public interface ISettingsService
{
    AppSettings CurrentSettings { get; }
    string CurrentStorageDirectory { get; }
    event EventHandler<string>? StorageDirectoryChanged;
    Task<AppSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
    Task<bool> ChangeStorageDirectoryAsync(string newDirectory);
    Task ResetStorageDirectoryAsync();
}

public class SettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static readonly string DefaultStorageDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PlannerSpace");

    private static readonly string BootstrapConfigPath = Path.Combine(
        DefaultStorageDirectory,
        "bootstrap_storage.json");

    private string _activeStorageDirectory;
    private string _settingsPath;
    private AppSettings _currentSettings = new();

    public AppSettings CurrentSettings => _currentSettings;
    public string CurrentStorageDirectory => _activeStorageDirectory;
    public event EventHandler<string>? StorageDirectoryChanged;

    public SettingsService()
    {
        Directory.CreateDirectory(DefaultStorageDirectory);
        _activeStorageDirectory = ResolveStorageDirectory();
        Directory.CreateDirectory(_activeStorageDirectory);
        _settingsPath = Path.Combine(_activeStorageDirectory, "settings.json");
    }

    private static string ResolveStorageDirectory()
    {
        if (File.Exists(BootstrapConfigPath))
        {
            try
            {
                string text = File.ReadAllText(BootstrapConfigPath);
                using var doc = JsonDocument.Parse(text);
                if (doc.RootElement.TryGetProperty("DataDirectory", out var prop))
                {
                    string? custom = prop.GetString();
                    if (!string.IsNullOrWhiteSpace(custom) && Directory.Exists(custom))
                    {
                        return custom;
                    }
                }
            }
            catch { }
        }
        return DefaultStorageDirectory;
    }

    public async Task<AppSettings> LoadSettingsAsync()
    {
        if (File.Exists(_settingsPath))
        {
            try
            {
                using var stream = File.OpenRead(_settingsPath);
                var loaded = await JsonSerializer.DeserializeAsync(stream, PlannerJsonContext.Default.AppSettings);
                if (loaded != null)
                {
                    _currentSettings = loaded;
                    return _currentSettings;
                }
            }
            catch
            {
                // Fallback to defaults
            }
        }

        _currentSettings = new AppSettings();
        await SaveSettingsAsync(_currentSettings);
        return _currentSettings;
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        _currentSettings = settings;
        string tempPath = _settingsPath + ".tmp";
        string? dir = Path.GetDirectoryName(_settingsPath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await JsonSerializer.SerializeAsync(fs, settings, PlannerJsonContext.Default.AppSettings);
        }

        File.Move(tempPath, _settingsPath, overwrite: true);
    }

    public async Task<bool> ChangeStorageDirectoryAsync(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory)) return false;
        if (string.Equals(_activeStorageDirectory, newDirectory, StringComparison.OrdinalIgnoreCase)) return true;

        try
        {
            Directory.CreateDirectory(newDirectory);

            string oldSettings = Path.Combine(_activeStorageDirectory, "settings.json");
            string newSettings = Path.Combine(newDirectory, "settings.json");
            if (File.Exists(oldSettings) && !File.Exists(newSettings))
            {
                File.Copy(oldSettings, newSettings);
            }

            string oldPlannerData = Path.Combine(_activeStorageDirectory, "planner_data.json");
            string newPlannerData = Path.Combine(newDirectory, "planner_data.json");
            if (File.Exists(oldPlannerData) && !File.Exists(newPlannerData))
            {
                File.Copy(oldPlannerData, newPlannerData);
            }

            string tmp = BootstrapConfigPath + ".tmp";
            string json = $"{{\"DataDirectory\":\"{newDirectory.Replace("\\", "\\\\")}\"}}";
            await File.WriteAllTextAsync(tmp, json);
            File.Move(tmp, BootstrapConfigPath, overwrite: true);

            _activeStorageDirectory = newDirectory;
            _settingsPath = Path.Combine(_activeStorageDirectory, "settings.json");

            await LoadSettingsAsync();
            StorageDirectoryChanged?.Invoke(this, _activeStorageDirectory);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task ResetStorageDirectoryAsync()
    {
        try
        {
            if (File.Exists(BootstrapConfigPath))
            {
                File.Delete(BootstrapConfigPath);
            }
        }
        catch { }

        _activeStorageDirectory = DefaultStorageDirectory;
        _settingsPath = Path.Combine(_activeStorageDirectory, "settings.json");
        await LoadSettingsAsync();
        StorageDirectoryChanged?.Invoke(this, _activeStorageDirectory);
    }
}
