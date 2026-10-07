using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Planner.ViewModels;

namespace Planner;

public sealed partial class MainWindow : Window
{
    public MainWindow(ElementTheme initialTheme = ElementTheme.Default)
    {
        InitializeComponent();

        if (Content is FrameworkElement root)
        {
            root.RequestedTheme = initialTheme;
        }
        RootFrame.RequestedTheme = initialTheme;

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");
        AppWindow.Title = "Planner";

        // Fixed sizing matching mockup ratio (1120 x 720)
        AppWindow.Resize(new Windows.Graphics.SizeInt32(1120, 720));

        if (AppWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
        }

        RootFrame.Navigate(typeof(MainPage));
    }
}
