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
    Task<AppSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
}

public class SettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _settingsPath;
    private AppSettings _currentSettings = new();

    public AppSettings CurrentSettings => _currentSettings;

    public SettingsService()
    {
        string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appDir = Path.Combine(baseDir, "PlannerSpace");
        Directory.CreateDirectory(appDir);
        _settingsPath = Path.Combine(appDir, "settings.json");
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
}
