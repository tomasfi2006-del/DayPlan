using System;
using System.Linq;
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
        RequestedTheme = ViewModel.CurrentTheme;
        Bindings.Update();

        // Setup XamlRoot for ContentDialogs
        if (XamlRoot != null)
        {
            NewListModalDialog.XamlRoot = XamlRoot;
            SettingsModalDialog.XamlRoot = XamlRoot;
            CreateTaskModalDialog.XamlRoot = XamlRoot;
            EditTaskModalDialog.XamlRoot = XamlRoot;
        }

        ViewModel.ThemeChanged += (s, theme) =>
        {
            RequestedTheme = theme;
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
            else if (args.PropertyName == nameof(ViewModel.IsEditTaskDialogOpen) && ViewModel.IsEditTaskDialogOpen)
            {
                if (EditTaskModalDialog.XamlRoot == null && XamlRoot != null)
                {
                    EditTaskModalDialog.XamlRoot = XamlRoot;
                }
                var result = await EditTaskModalDialog.ShowAsync();
                if (result != ContentDialogResult.Primary)
                {
                    ViewModel.CloseEditTaskDialog();
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

    private async void OnDeleteProjectContextClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is PlannerProject proj)
        {
            await ViewModel.DeleteProjectAsync(proj);
        }
    }

    private void OnFilterAllClicked(object sender, RoutedEventArgs e) => ViewModel.SetTagFilter("All");

    private void OnFilterTagChipClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PlannerTag tag)
        {
            ViewModel.SetTagFilter(tag.Name);
        }
    }

    private async void OnFilterTagMenuColorClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.Tag is string colorHex && item.DataContext is PlannerTag tag)
        {
            await ViewModel.UpdateTagColorAsync(tag.Name, colorHex);
        }
    }

    private async void OnDeleteFilterTagClicked(object sender, RoutedEventArgs e)
    {
        string? tagName = null;
        if (sender is MenuFlyoutItem item)
        {
            if (item.Tag is string tagStr) tagName = tagStr;
            else if (item.DataContext is PlannerTag tg) tagName = tg.Name;
        }

        if (!string.IsNullOrWhiteSpace(tagName))
        {
            await ViewModel.DeleteTagFilterAsync(tagName);
        }
    }

    private string _newFilterTagColor = "#005FB8";
    private void OnNewFilterColorPicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string hex)
        {
            _newFilterTagColor = hex;
        }
    }

    private async void OnCreateNewFilterTagClicked(object sender, RoutedEventArgs e)
    {
        if (NewFilterTagNameBox != null && !string.IsNullOrWhiteSpace(NewFilterTagNameBox.Text))
        {
            string name = NewFilterTagNameBox.Text.Trim();
            NewFilterTagNameBox.Text = string.Empty;
            NewFilterFlyout?.Hide();
            await ViewModel.AddNewFilterTagAsync(name, _newFilterTagColor);
        }
    }

    private void OnNewFilterTagKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            OnCreateNewFilterTagClicked(sender, e);
        }
    }

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

    private void OnCtrlFInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        GlobalSearchBox.Focus(FocusState.Programmatic);
        args.Handled = true;
    }

    private void OnCtrlTInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        ViewModel.SelectNav("Today");
        args.Handled = true;
    }

    private void OnCtrlIInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        ViewModel.SelectNav("Inbox");
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

    private async void OnEditTaskConfirmed(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.EditTaskTitle))
        {
            args.Cancel = true;
            return;
        }

        var deferral = args.GetDeferral();
        try
        {
            await ViewModel.ConfirmEditTaskAsync();
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

    private void OnQuickTagWorkClicked(object sender, RoutedEventArgs e) => ViewModel.AddTagToDraft("#Work");
    private void OnQuickTagPersonalClicked(object sender, RoutedEventArgs e) => ViewModel.AddTagToDraft("#Personal");
    private void OnQuickTagFocusClicked(object sender, RoutedEventArgs e) => ViewModel.AddTagToDraft("#Focus");

    private void OnEditQuickTagWorkClicked(object sender, RoutedEventArgs e) => ViewModel.AddTagToEdit("#Work");
    private void OnEditQuickTagPersonalClicked(object sender, RoutedEventArgs e) => ViewModel.AddTagToEdit("#Personal");
    private void OnEditQuickTagFocusClicked(object sender, RoutedEventArgs e) => ViewModel.AddTagToEdit("#Focus");

    private void OnEditSchedulePresetTodayClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.EditTaskDueDate = DateTimeOffset.Now;
    }

    private void OnEditSchedulePresetTomorrowClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.EditTaskDueDate = DateTimeOffset.Now.AddDays(1);
    }

    private void OnEditSchedulePresetMorningClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.EditTaskDueTimeText = "9:00 AM";
        ViewModel.EditTaskSection = "Morning";
    }

    private void OnEditSchedulePresetAfternoonClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.EditTaskDueTimeText = "2:00 PM";
        ViewModel.EditTaskSection = "Afternoon";
    }

    private void OnColorSwatchClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string hex)
        {
            ViewModel.TaskDraftTagColor = hex;
        }
    }

    private void OnEditColorSwatchClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string hex)
        {
            ViewModel.EditTaskTagColor = hex;
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

    private void OnTaskEditRequested(object? sender, PlannerTask task)
    {
        ViewModel.OpenEditTaskDialog(task);
    }

    private async void OnSubTaskAddRequested(object? sender, PlannerTask task)
    {
        await ViewModel.AddSubTaskAsync(task);
    }

    private async void OnSubTaskToggleRequested(object? sender, (string TaskId, string SubTaskId) e)
    {
        await ViewModel.ToggleSubTaskAsync(e.TaskId, e.SubTaskId);
    }

    private async void OnSubTaskDeleteRequested(object? sender, (string TaskId, string SubTaskId) e)
    {
        await ViewModel.DeleteSubTaskAsync(e.TaskId, e.SubTaskId);
    }

    private async void OnSubTaskCreateRequested(object? sender, (string TaskId, string Title) e)
    {
        await ViewModel.AddNamedSubTaskAsync(e.TaskId, e.Title);
    }

    private async void OnCreateListConfirmed(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.NewListName))
        {
            args.Cancel = true;
            return;
        }

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

        bool isDark = ViewModel.IsDarkTheme;
        return isDark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x1E, 0x29, 0x3B))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0xE2, 0xE8, 0xF0));
    }

    private Brush GetNavTextForeground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        bool isDark = ViewModel.IsDarkTheme;
        if (isSelected)
        {
            return isDark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x60, 0xCD, 0xFF))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x00, 0x5F, 0xB8));
        }

        return isDark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x94, 0xA3, 0xB8))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0x33, 0x41, 0x55));
    }

    private Brush GetAllFilterBackground(string currentFilter)
    {
        bool isSelected = string.Equals(currentFilter, "All", StringComparison.OrdinalIgnoreCase);
        if (!isSelected)
        {
            return new SolidColorBrush(Colors.Transparent);
        }

        bool isDark = ViewModel.IsDarkTheme;
        return isDark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x2A, 0x34, 0x41))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0xFF, 0xFF, 0xFF));
    }

    private Brush GetAllFilterForeground(string currentFilter)
    {
        bool isSelected = string.Equals(currentFilter, "All", StringComparison.OrdinalIgnoreCase);
        bool isDark = ViewModel.IsDarkTheme;
        if (isSelected)
        {
            return isDark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0xF8, 0xFA, 0xFC))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x0F, 0x17, 0x2A));
        }

        return isDark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x94, 0xA3, 0xB8))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0x47, 0x55, 0x69));
    }

    private Brush GetSegmentBackground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (!isSelected)
        {
            return new SolidColorBrush(Colors.Transparent);
        }

        bool isDark = ViewModel.IsDarkTheme;
        return isDark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x2A, 0x34, 0x41))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0xFF, 0xFF, 0xFF));
    }

    private Brush GetSegmentForeground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        bool isDark = ViewModel.IsDarkTheme;
        if (isSelected)
        {
            return isDark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0xF8, 0xFA, 0xFC))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x0F, 0x17, 0x2A));
        }

        return isDark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x94, 0xA3, 0xB8))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0x47, 0x55, 0x69));
    }

    private Brush GetTagBackground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (isSelected)
        {
            return ViewModel.IsDarkTheme
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x60, 0xCD, 0xFF))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x00, 0x5F, 0xB8));
        }

        return ViewModel.IsDarkTheme
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x1C, 0x22, 0x30))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0xFF, 0xFF, 0xFF));
    }

    private Brush GetTagForeground(string current, string target)
    {
        bool isSelected = string.Equals(current, target, StringComparison.OrdinalIgnoreCase);
        if (isSelected)
        {
            return ViewModel.IsDarkTheme
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x0F, 0x17, 0x2A))
                : new SolidColorBrush(Colors.White);
        }

        return ViewModel.IsDarkTheme
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x94, 0xA3, 0xB8))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0x47, 0x55, 0x69));
    }

    private void OnTagDropdownSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            ViewModel.FilterDropdownTags(tb.Text);
        }
    }

    private void OnDropdownTagItemClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is PlannerTag tag)
        {
            ViewModel.SelectTagFilter(tag.Name);
        }
    }

    private void OnDropdownTagAllClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.ClearTagFilter();
    }

    private void OnClearTagFilterClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.ClearTagFilter();
    }

    private async void OnBrowseStorageFolderClicked(object sender, RoutedEventArgs e)
    {
        var picker = new Windows.Storage.Pickers.FolderPicker();
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add("*");

        nint hwnd = App.WindowHandle;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var folder = await picker.PickSingleFolderAsync();
        if (folder != null)
        {
            await ViewModel.ChangeStorageLocationAsync(folder.Path);
        }
    }

    private async void OnResetStorageLocationClicked(object sender, RoutedEventArgs e)
    {
        await ViewModel.ResetStorageLocationAsync();
    }

    private string GetCountLabel(int count)
    {
        return $"{count} {(count == 1 ? "item" : "items")}";
    }

    // Area & List Preset and Custom Creation Handlers
    private async void OnAreaPresetClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string preset)
        {
            NewAreaFlyout.Hide();
            await ViewModel.CreateAreaAsync(preset);
        }
    }

    private async void OnCreateCustomAreaClicked(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(CustomAreaNameBox.Text))
        {
            string name = CustomAreaNameBox.Text.Trim();
            CustomAreaNameBox.Text = string.Empty;
            NewAreaFlyout.Hide();
            await ViewModel.CreateAreaAsync(name);
        }
    }

    private async void OnCustomAreaKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter && !string.IsNullOrWhiteSpace(CustomAreaNameBox.Text))
        {
            e.Handled = true;
            string name = CustomAreaNameBox.Text.Trim();
            CustomAreaNameBox.Text = string.Empty;
            NewAreaFlyout.Hide();
            await ViewModel.CreateAreaAsync(name);
        }
    }

    private async void OnDeleteAreaContextClicked(object sender, RoutedEventArgs e)
    {
        string? areaName = null;
        if (sender is MenuFlyoutItem item)
        {
            if (item.Tag is string tagStr && !string.IsNullOrWhiteSpace(tagStr))
            {
                areaName = tagStr;
            }
            else if (item.DataContext is ProjectAreaGroup group)
            {
                areaName = group.AreaName;
            }
        }
        else if (sender is FrameworkElement fe && fe.Tag is string tagStr && !string.IsNullOrWhiteSpace(tagStr))
        {
            areaName = tagStr;
        }

        if (!string.IsNullOrWhiteSpace(areaName))
        {
            await ViewModel.DeleteAreaAsync(areaName);
        }
    }

    private async void OnListPresetClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            string? areaName = (btn.DataContext as ProjectAreaGroup)?.AreaName;
            if (string.IsNullOrWhiteSpace(areaName) && btn.Parent is StackPanel sp && sp.Parent is StackPanel spRoot && spRoot.Tag is string tag)
            {
                areaName = tag;
            }
            if (!string.IsNullOrWhiteSpace(areaName) && btn.Tag is string preset)
            {
                await ViewModel.CreateListInAreaAsync(areaName, preset);
            }
        }
    }

    private async void OnCreateCustomListClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            string? areaName = (btn.DataContext as ProjectAreaGroup)?.AreaName;
            if (string.IsNullOrWhiteSpace(areaName) && btn.Parent is StackPanel spTag && spTag.Tag is string tag)
            {
                areaName = tag;
            }
            if (btn.Parent is StackPanel sp && sp.Children.OfType<TextBox>().FirstOrDefault() is TextBox tb && !string.IsNullOrWhiteSpace(tb.Text))
            {
                string name = tb.Text.Trim();
                tb.Text = string.Empty;
                if (!string.IsNullOrWhiteSpace(areaName))
                {
                    await ViewModel.CreateListInAreaAsync(areaName, name);
                }
            }
        }
    }

    private async void OnCustomListKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter && sender is TextBox tb && !string.IsNullOrWhiteSpace(tb.Text))
        {
            string? areaName = (tb.DataContext as ProjectAreaGroup)?.AreaName;
            if (string.IsNullOrWhiteSpace(areaName) && tb.Parent is StackPanel sp && sp.Tag is string tag)
            {
                areaName = tag;
            }
            if (!string.IsNullOrWhiteSpace(areaName))
            {
                e.Handled = true;
                string name = tb.Text.Trim();
                tb.Text = string.Empty;
                await ViewModel.CreateListInAreaAsync(areaName, name);
            }
        }
    }

    // Multi-tag Chip Handlers for Create Modal
    private void OnDraftTagInputKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            e.Handled = true;
            if (!string.IsNullOrWhiteSpace(DraftTagInputBox.Text))
            {
                ViewModel.AddDraftTag(DraftTagInputBox.Text);
                DraftTagInputBox.Text = string.Empty;
            }
        }
    }

    private void OnAddDraftTagButtonClicked(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(DraftTagInputBox.Text))
        {
            ViewModel.AddDraftTag(DraftTagInputBox.Text);
            DraftTagInputBox.Text = string.Empty;
        }
    }

    private void OnRemoveDraftTagClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is PlannerTag tag)
        {
            ViewModel.RemoveDraftTag(tag);
        }
    }

    private void OnDraftTagChipColorClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PlannerTag tag && btn.Tag is string color)
        {
            tag.ColorHex = color;
            ViewModel.UpdateDraftTagColor(tag, color);
        }
    }

    // Checklist / Subtask Handlers for Create Modal
    private void OnDraftSubtaskInputKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            e.Handled = true;
            if (!string.IsNullOrWhiteSpace(DraftSubtaskInputBox.Text))
            {
                ViewModel.AddDraftSubtask(DraftSubtaskInputBox.Text);
                DraftSubtaskInputBox.Text = string.Empty;
            }
        }
    }

    private void OnAddDraftSubtaskButtonClicked(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(DraftSubtaskInputBox.Text))
        {
            ViewModel.AddDraftSubtask(DraftSubtaskInputBox.Text);
            DraftSubtaskInputBox.Text = string.Empty;
        }
    }

    private void OnRemoveDraftSubtaskClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is PlannerSubTask subtask)
        {
            ViewModel.RemoveDraftSubtask(subtask);
        }
    }

    // Multi-tag Chip Handlers for Edit Modal
    private void OnEditTagInputKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            e.Handled = true;
            if (!string.IsNullOrWhiteSpace(EditTagInputBox.Text))
            {
                ViewModel.AddEditTag(EditTagInputBox.Text);
                EditTagInputBox.Text = string.Empty;
            }
        }
    }

    private void OnAddEditTagButtonClicked(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(EditTagInputBox.Text))
        {
            ViewModel.AddEditTag(EditTagInputBox.Text);
            EditTagInputBox.Text = string.Empty;
        }
    }

    private void OnRemoveEditTagClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is PlannerTag tag)
        {
            ViewModel.RemoveEditTag(tag);
        }
    }

    private void OnEditTagChipColorClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is PlannerTag tag && btn.Tag is string color)
        {
            tag.ColorHex = color;
            ViewModel.UpdateEditTagColor(tag, color);
        }
    }

    // Checklist / Subtask Handlers for Edit Modal
    private void OnEditSubtaskInputKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            e.Handled = true;
            if (!string.IsNullOrWhiteSpace(EditSubtaskInputBox.Text))
            {
                ViewModel.AddEditSubtask(EditSubtaskInputBox.Text);
                EditSubtaskInputBox.Text = string.Empty;
            }
        }
    }

    private void OnAddEditSubtaskButtonClicked(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(EditSubtaskInputBox.Text))
        {
            ViewModel.AddEditSubtask(EditSubtaskInputBox.Text);
            EditSubtaskInputBox.Text = string.Empty;
        }
    }

    private void OnRemoveEditSubtaskClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is PlannerSubTask subtask)
        {
            ViewModel.RemoveEditSubtask(subtask);
        }
    }
}
