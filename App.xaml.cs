using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Planner.Native;
using Planner.Services;
using Planner.ViewModels;

namespace Planner;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private static Mutex? _singleInstanceMutex;
    private const string AppMutexName = "Global\\Planner_SingleInstance_Mutex_38384";

    public static IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// The main application window.
    /// </summary>
    public static Window Window { get; private set; } = null!;

    /// <summary>
    /// The UI thread dispatcher.
    /// </summary>
    public static Microsoft.UI.Dispatching.DispatcherQueue DispatcherQueue { get; private set; } = null!;

    /// <summary>
    /// The native window handle (HWND).
    /// </summary>
    public static nint WindowHandle =>
        WinRT.Interop.WindowNative.GetWindowHandle(Window);

    public App()
    {
        InitializeComponent();

        _singleInstanceMutex = new Mutex(true, AppMutexName, out bool isNewInstance);
        if (!isNewInstance)
        {
            // Another instance is already running
        }

        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register core services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IPlannerDataService, PlannerDataService>();

        // Register ViewModels
        services.AddSingleton<MainViewModel>();

        Services = services.BuildServiceProvider();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        var settingsService = Services.GetRequiredService<ISettingsService>();
        var settings = await settingsService.LoadSettingsAsync();

        Window = new MainWindow(settings.Theme);

        if (Window.Content is FrameworkElement root)
        {
            root.RequestedTheme = settings.Theme;
        }

        Window.Activate();

        Win32Interop.SetImmersiveDarkMode(WindowHandle, settings.Theme == ElementTheme.Dark);
    }
}
