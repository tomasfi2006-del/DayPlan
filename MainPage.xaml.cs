using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Planner.Models;
using Planner.ViewModels;

namespace Planner;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel => App.Services.GetRequiredService<MainViewModel>();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitializeAsync();

        // Setup XamlRoot for ContentDialogs
        if (XamlRoot != null)
        {
            NewListModalDialog.XamlRoot = XamlRoot;
            SettingsModalDialog.XamlRoot = XamlRoot;
            CreateTaskModalDialog.XamlRoot = XamlRoot;
        }

        ViewModel.ThemeChanged += (s, theme) =>
        {
            Bindings.Update();
        };

        ViewModel.PropertyChanged += async (s, args) =>
        {
            if (args.PropertyName == nameof(ViewModel.IsSettingsOpen) && ViewModel.IsSettingsOpen)
            {
                if (SettingsModalDialog.XamlRoot == null && XamlRoot != null)
                {
                    SettingsModalDialog.XamlRoot = XamlRoot;
                }
                await SettingsModalDialog.ShowAsync();
                ViewModel.CloseSettings();
            }
            else if (args.PropertyName == nameof(ViewModel.IsNewListDialogOpen) && ViewModel.IsNewListDialogOpen)
            {
                if (NewListModalDialog.XamlRoot == null && XamlRoot != null)
                {
                    NewListModalDialog.XamlRoot = XamlRoot;
                }
                await NewListModalDialog.ShowAsync();
            }
            else if (args.PropertyName == nameof(ViewModel.IsCreateTaskDialogOpen) && ViewModel.IsCreateTaskDialogOpen)
            {
                if (CreateTaskModalDialog.XamlRoot == null && XamlRoot != null)
                {
                    CreateTaskModalDialog.XamlRoot = XamlRoot;
                }
                var result = await CreateTaskModalDialog.ShowAsync();
                if (result != ContentDialogResult.Primary)
                {
                    ViewModel.CloseCreateTaskDialog();
                }
            }
        };
    }

    private void OnNavInboxClicked(object sender, RoutedEventArgs e) => ViewModel.SelectNav("Inbox");
    private void OnNavTodayClicked(object sender, RoutedEventArgs e) => ViewModel.SelectNav("Today");
    private void OnNavUpcomingClicked(object sender, RoutedEventArgs e) => ViewModel.SelectNav("Upcoming");
    private void OnNavAnytimeClicked(object sender, RoutedEventArgs e) => ViewModel.SelectNav("Anytime");
    private void OnNavSomedayClicked(object sender, RoutedEventArgs e) => ViewModel.SelectNav("Someday");
    private void OnNavLogbookClicked(object sender, RoutedEventArgs e) => ViewModel.SelectNav("Logbook");

    private void OnProjectItemClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PlannerProject proj)
        {
            ViewModel.SelectProject(proj);
        }
    }

    private void OnFilterAllClicked(object sender, RoutedEventArgs e) => ViewModel.SetSegmentFilter("All");
    private void OnFilterWorkClicked(object sender, RoutedEventArgs e) => ViewModel.SetSegmentFilter("Work");
    private void OnFilterPersonalClicked(object sender, RoutedEventArgs e) => ViewModel.SetSegmentFilter("Personal");
    private void OnFilterFocusClicked(object sender, RoutedEventArgs e) => ViewModel.SetSegmentFilter("Focus");
    private void OnFilterQuickClicked(object sender, RoutedEventArgs e) => ViewModel.SetSegmentFilter("Quick 15m");

    private void OnTagAllClicked(object sender, RoutedEventArgs e) => ViewModel.SetTagFilter("All");

    private void OnTagChipClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PlannerTag tag)
        {
            ViewModel.SetTagFilter(tag.Name);
        }
    }

    private void OnCtrlNInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        ViewModel.OpenCreateTaskDialog();
        args.Handled = true;
    }

    private void OnCtrlNBadgeTapped(object sender, TappedRoutedEventArgs e)
    {
        ViewModel.OpenCreateTaskDialog();
    }

    private async void OnCreateTaskConfirmed(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.TaskDraftTitle))
        {
            args.Cancel = true;
            return;
        }

        var deferral = args.GetDeferral();
        try
        {
            await ViewModel.ConfirmCreateTaskAsync();
        }
        finally
        {
            deferral.Complete();
        }
    }

    private void OnModalSectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item)
        {
            ViewModel.TaskDraftSection = item.Content?.ToString() ?? "Morning";
        }
    }

    private void OnSchedulePresetTodayClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.TaskDraftDueDate = DateTimeOffset.Now;
    }

    private void OnSchedulePresetTomorrowClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.TaskDraftDueDate = DateTimeOffset.Now.AddDays(1);
    }

    private void OnSchedulePresetMorningClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.TaskDraftDueTimeText = "9:00 AM";
        ViewModel.TaskDraftSection = "Morning";
    }

    private void OnSchedulePresetAfternoonClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.TaskDraftDueTimeText = "2:00 PM";
        ViewModel.TaskDraftSection = "Afternoon";
    }

    private void OnColorSwatchClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string hex)
        {
            ViewModel.TaskDraftTagColor = hex;
        }
    }

    private async void OnTaskToggleRequested(object? sender, PlannerTask task)
    {
        await ViewModel.ToggleTaskCompletionAsync(task);
    }

    private async void OnTaskDeleteRequested(object? sender, PlannerTask task)
    {
        await ViewModel.DeleteTaskAsync(task);
    }

    private async void OnSubTaskAddRequested(object? sender, PlannerTask task)
    {
        await ViewModel.AddSubTaskAsync(task);
    }

    private async void OnCreateListConfirmed(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (AreaSelectComboBox.SelectedItem is ComboBoxItem item)
        {
            ViewModel.NewListArea = item.Content?.ToString() ?? "Work";
        }
        await ViewModel.CreateNewListAsync();
    }

    private async void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb)
        {
            await ViewModel.ChangeThemeAsync(cb.SelectedIndex);
        }
    }

    private Brush GetNavBackground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (!isSelected)
        {
            return new SolidColorBrush(Colors.Transparent);
        }

        return ActualTheme == ElementTheme.Dark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x1E, 0x29, 0x3B))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0xFF, 0xFF, 0xFF));
    }

    private Brush GetNavTextForeground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (isSelected)
        {
            return ActualTheme == ElementTheme.Dark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x60, 0xCD, 0xFF))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x00, 0x5F, 0xB8));
        }

        return ActualTheme == ElementTheme.Dark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0xCB, 0xD5, 0xE1))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0x1E, 0x29, 0x3B));
    }

    private Brush GetSegmentBackground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (!isSelected)
        {
            return new SolidColorBrush(Colors.Transparent);
        }

        return ActualTheme == ElementTheme.Dark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x2A, 0x34, 0x41))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0xFF, 0xFF, 0xFF));
    }

    private Brush GetSegmentForeground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (isSelected)
        {
            return ActualTheme == ElementTheme.Dark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0xF8, 0xFA, 0xFC))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x0F, 0x17, 0x2A));
        }

        return ActualTheme == ElementTheme.Dark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x94, 0xA3, 0xB8))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0x64, 0x74, 0x8B));
    }

    private Brush GetTagBackground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (isSelected)
        {
            return ActualTheme == ElementTheme.Dark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x60, 0xCD, 0xFF))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x00, 0x5F, 0xB8));
        }

        return ActualTheme == ElementTheme.Dark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x1C, 0x22, 0x30))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0xFF, 0xFF, 0xFF));
    }

    private Brush GetTagForeground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (isSelected)
        {
            return ActualTheme == ElementTheme.Dark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x0F, 0x17, 0x2A))
                : new SolidColorBrush(Colors.White);
        }

        return ActualTheme == ElementTheme.Dark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x94, 0xA3, 0xB8))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0x47, 0x55, 0x69));
    }

    private string GetCountLabel(int count)
    {
        return $"{count} {(count == 1 ? "item" : "items")}";
    }
}
