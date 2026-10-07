using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Planner.Models;
using Planner.Services;

namespace Planner.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IPlannerDataService _dataService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    public partial string SelectedNavCategory { get; set; } = "Today";

    [ObservableProperty]
    public partial string? SelectedProjectId { get; set; }

    [ObservableProperty]
    public partial string SelectedProjectName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SegmentFilter { get; set; } = "All"; // All, Work, Personal, Focus, Quick 15m

    [ObservableProperty]
    public partial string TagFilter { get; set; } = "All"; // All, #Design, #Strategy, #Deep Work, #Personal

    [ObservableProperty]
    public partial string CurrentDateText { get; set; } = DateTime.Now.ToString("dddd, MMMM d");

    [ObservableProperty]
    public partial string HeaderTitle { get; set; } = "Today";

    [ObservableProperty]
    public partial string HeaderSubtitle { get; set; } = DateTime.Now.ToString("dddd, MMMM d");

    [ObservableProperty]
    public partial string NewTaskTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int CompletedTasksCount { get; set; }

    [ObservableProperty]
    public partial int TotalTasksCount { get; set; }

    [ObservableProperty]
    public partial double CompletionPercentage { get; set; }

    [ObservableProperty]
    public partial string CompletionRatioText { get; set; } = "0 of 0 completed (0%)";

    [ObservableProperty]
    public partial int InboxBadgeCount { get; set; }

    [ObservableProperty]
    public partial bool IsTodayView { get; set; } = true;

    [ObservableProperty]
    public partial bool IsProjectView { get; set; }

    [ObservableProperty]
    public partial bool IsInboxView { get; set; }

    [ObservableProperty]
    public partial bool IsUpcomingView { get; set; }

    [ObservableProperty]
    public partial bool IsLogbookView { get; set; }

    [ObservableProperty]
    public partial bool IsGenericListView { get; set; }

    [ObservableProperty]
    public partial string ProfileName { get; set; } = "Planner Space";

    [ObservableProperty]
    public partial string ProfilePlan { get; set; } = "Personal Plan";

    [ObservableProperty]
    public partial string AvatarUri { get; set; } = "ms-appx:///Assets/profile_avatar.png";

    [ObservableProperty]
    public partial bool IsSettingsOpen { get; set; }

    [ObservableProperty]
    public partial int SelectedThemeIndex { get; set; } // 0: Light, 1: Dark, 2: Default

    [ObservableProperty]
    public partial bool ScrubberVisible { get; set; }

    [ObservableProperty]
    public partial double ScrubberX { get; set; }

    [ObservableProperty]
    public partial string ScrubberDayText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ScrubberCountText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewListName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewListArea { get; set; } = "Work";

    [ObservableProperty]
    public partial bool IsNewListDialogOpen { get; set; }

    [ObservableProperty]
    public partial bool HasWorkProjects { get; set; }

    [ObservableProperty]
    public partial bool HasPersonalProjects { get; set; }

    [ObservableProperty]
    public partial bool HasNoProjects { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSidebarExpanded { get; set; } = true;

    [ObservableProperty]
    public partial bool IsCreateTaskDialogOpen { get; set; }

    [ObservableProperty]
    public partial string TaskDraftTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TaskDraftSection { get; set; } = "Morning";

    [ObservableProperty]
    public partial DateTimeOffset? TaskDraftDueDate { get; set; }

    [ObservableProperty]
    public partial string TaskDraftDueTimeText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TaskDraftTag { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TaskDraftTagColor { get; set; } = "#005FB8";

    [ObservableProperty]
    public partial string TaskDraftNotes { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TaskDraftDuration { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TaskDraftProjectId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TaskDraftProjectName { get; set; } = string.Empty;

    // Edit Task State
    [ObservableProperty]
    public partial bool IsEditTaskDialogOpen { get; set; }

    [ObservableProperty]
    public partial string EditTaskId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditTaskTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditTaskSection { get; set; } = "Morning";

    [ObservableProperty]
    public partial DateTimeOffset? EditTaskDueDate { get; set; }

    [ObservableProperty]
    public partial string EditTaskDueTimeText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditTaskDuration { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditTaskTag { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditTaskTagColor { get; set; } = "#005FB8";

    [ObservableProperty]
    public partial string EditTaskNotes { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditTaskProjectId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditTaskProjectName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WeeklyTotalTasksText { get; set; } = "0 Tasks Total";

    public ObservableCollection<PlannerTag> AvailableTags { get; } = new();

    public ObservableCollection<PlannerTask> MorningTasks { get; } = new();
    public ObservableCollection<PlannerTask> AfternoonTasks { get; } = new();
    public ObservableCollection<PlannerTask> EveningTasks { get; } = new();
    public ObservableCollection<PlannerTask> SectionedTasks { get; } = new();

    public ObservableCollection<PlannerProject> Projects { get; } = new();
    public ObservableCollection<PlannerProject> WorkProjects { get; } = new();
    public ObservableCollection<PlannerProject> PersonalProjects { get; } = new();

    public ObservableCollection<CalendarHorizonEvent> HorizonEvents { get; } = new();
    public ObservableCollection<WeeklyDayLoad> WeeklyLoads { get; } = new();

    public event EventHandler<ElementTheme>? ThemeChanged;

    private PlannerDataPackage? _currentPackage;

    public MainViewModel(IPlannerDataService dataService, ISettingsService settingsService)
    {
        _dataService = dataService;
        _settingsService = settingsService;
    }

    public async Task InitializeAsync()
    {
        var settings = await _settingsService.LoadSettingsAsync();
        ProfileName = settings.ProfileName;
        ProfilePlan = settings.ProfilePlan;
        AvatarUri = settings.AvatarPath;
        SelectedThemeIndex = settings.Theme == ElementTheme.Light ? 0 : (settings.Theme == ElementTheme.Dark ? 1 : 2);

        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task RefreshDataAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var expandedIds = MorningTasks.Concat(AfternoonTasks).Concat(EveningTasks).Concat(SectionedTasks)
            .Where(t => t.IsExpanded)
            .Select(t => t.Id)
            .ToHashSet();

        _currentPackage = await _dataService.LoadDataAsync();

        foreach (var t in _currentPackage.Tasks)
        {
            if (expandedIds.Contains(t.Id))
            {
                t.IsExpanded = true;
            }
        }

        Projects.Clear();
        WorkProjects.Clear();
        PersonalProjects.Clear();
        foreach (var p in _currentPackage.Projects)
        {
            Projects.Add(p);
            if (p.Area == "Work") WorkProjects.Add(p);
            else PersonalProjects.Add(p);
        }

        HasWorkProjects = WorkProjects.Count > 0;
        HasPersonalProjects = PersonalProjects.Count > 0;
        HasNoProjects = Projects.Count == 0;

        AvailableTags.Clear();
        foreach (var tag in _currentPackage.Tags)
        {
            AvailableTags.Add(tag);
        }

        // 1. Populate Live Today's Horizon from timed tasks
        HorizonEvents.Clear();
        var todayTimedTasks = _currentPackage.Tasks
            .Where(t => !t.IsCompleted && (t.Section is "Morning" or "Afternoon" or "Evening" || (t.DueDate.HasValue && t.DueDate.Value.Date == DateTime.Today)))
            .Where(t => !string.IsNullOrWhiteSpace(t.DueTimeText))
            .ToList();

        foreach (var t in todayTimedTasks)
        {
            HorizonEvents.Add(new CalendarHorizonEvent
            {
                Id = t.Id,
                Title = t.Title,
                TimeRange = t.DueTimeText,
                AccentColor = !string.IsNullOrEmpty(t.TagColorHex) ? t.TagColorHex : (!string.IsNullOrEmpty(t.ProjectColor) ? t.ProjectColor : "#005FB8"),
                Location = t.ProjectName
            });
        }

        // 2. Compute dynamic Weekly Loads (Monday through Sunday)
        DateTime today = DateTime.Today;
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime startOfWeek = today.AddDays(-1 * diff).Date;

        WeeklyLoads.Clear();
        int weeklyTotal = 0;
        string[] dayShorts = ["M", "T", "W", "T", "F", "S", "S"];
        string[] dayNames = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

        for (int i = 0; i < 7; i++)
        {
            DateTime currentDay = startOfWeek.AddDays(i);
            int count = _currentPackage.Tasks.Count(t =>
                (t.DueDate.HasValue && t.DueDate.Value.Date == currentDay) ||
                (t.CompletedAt.HasValue && t.CompletedAt.Value.Date == currentDay) ||
                (currentDay == today && t.Section is "Morning" or "Afternoon" or "Evening"));

            weeklyTotal += count;
            double barHeight = Math.Min(64, Math.Max(4, count * 7.0));

            WeeklyLoads.Add(new WeeklyDayLoad
            {
                DayShort = dayShorts[i],
                DayName = dayNames[i],
                TaskCount = count,
                BarHeight = barHeight,
                IsToday = currentDay == today
            });
        }

        WeeklyTotalTasksText = $"{weeklyTotal} {(weeklyTotal == 1 ? "Task Total" : "Tasks Total")}";

        UpdateViewFilters();
    }

    private void UpdateViewFilters()
    {
        if (_currentPackage == null) return;

        InboxBadgeCount = _currentPackage.Tasks.Count(t => t.Section == "Inbox" && !t.IsCompleted);

        IsTodayView = SelectedNavCategory == "Today";
        IsInboxView = SelectedNavCategory == "Inbox";
        IsUpcomingView = SelectedNavCategory == "Upcoming";
        IsLogbookView = SelectedNavCategory == "Logbook";
        IsProjectView = SelectedNavCategory == "Project";
        IsGenericListView = SelectedNavCategory is "Anytime" or "Someday";

        switch (SelectedNavCategory)
        {
            case "Today":
                HeaderTitle = "Today";
                HeaderSubtitle = DateTime.Now.ToString("dddd, MMMM d");
                break;
            case "Inbox":
                HeaderTitle = "Inbox";
                HeaderSubtitle = $"{InboxBadgeCount} items needing triage";
                break;
            case "Upcoming":
                HeaderTitle = "Upcoming";
                HeaderSubtitle = "Upcoming schedules and deadlines";
                break;
            case "Anytime":
                HeaderTitle = "Anytime";
                HeaderSubtitle = "Tasks ready to start whenever";
                break;
            case "Someday":
                HeaderTitle = "Someday";
                HeaderSubtitle = "Ideas and long-term backlog";
                break;
            case "Logbook":
                HeaderTitle = "Logbook";
                HeaderSubtitle = "Archived and completed items";
                break;
            case "Project":
                HeaderTitle = SelectedProjectName;
                HeaderSubtitle = "Project view";
                break;
        }

        var sourceTasks = _currentPackage.Tasks.AsEnumerable();

        // 1. Navigation Category filter
        if (IsTodayView)
        {
            sourceTasks = sourceTasks.Where(t => t.Section is "Morning" or "Afternoon" or "Evening");
        }
        else if (IsInboxView)
        {
            sourceTasks = sourceTasks.Where(t => t.Section == "Inbox");
        }
        else if (IsUpcomingView)
        {
            sourceTasks = sourceTasks.Where(t => t.Section == "Upcoming" || (t.DueDate.HasValue && t.DueDate.Value.Date > DateTime.Today));
        }
        else if (IsLogbookView)
        {
            sourceTasks = sourceTasks.Where(t => t.IsCompleted);
        }
        else if (IsGenericListView)
        {
            sourceTasks = sourceTasks.Where(t => t.Section == SelectedNavCategory);
        }
        else if (IsProjectView && !string.IsNullOrEmpty(SelectedProjectId))
        {
            sourceTasks = sourceTasks.Where(t => t.ProjectId == SelectedProjectId);
        }

        // 2. Segment Filter (All, Work, Personal, Focus, Quick 15m)
        if (SegmentFilter == "Work")
        {
            var workProjIds = Projects.Where(p => p.Area == "Work").Select(p => p.Id).ToHashSet();
            sourceTasks = sourceTasks.Where(t =>
                (!string.IsNullOrEmpty(t.ProjectId) && workProjIds.Contains(t.ProjectId)) ||
                t.PrimaryTag is "#Dev" or "#Strategy" or "#Design" or "#Work");
        }
        else if (SegmentFilter == "Personal")
        {
            var personalProjIds = Projects.Where(p => p.Area == "Personal").Select(p => p.Id).ToHashSet();
            sourceTasks = sourceTasks.Where(t =>
                (!string.IsNullOrEmpty(t.ProjectId) && personalProjIds.Contains(t.ProjectId)) ||
                t.PrimaryTag is "#Personal" or "#Habits" or "#Home");
        }
        else if (SegmentFilter == "Focus")
        {
            sourceTasks = sourceTasks.Where(t => t.PrimaryTag.Contains("Focus", StringComparison.OrdinalIgnoreCase) || t.Section == "Morning");
        }
        else if (SegmentFilter == "Quick 15m")
        {
            sourceTasks = sourceTasks.Where(t => t.DurationText.Contains("15", StringComparison.OrdinalIgnoreCase));
        }

        // 3. Tag Filter
        if (TagFilter != "All")
        {
            sourceTasks = sourceTasks.Where(t => t.PrimaryTag.Equals(TagFilter, StringComparison.OrdinalIgnoreCase));
        }

        // 4. Search Filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string q = SearchText.Trim();
            sourceTasks = sourceTasks.Where(t =>
                t.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                t.Notes.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                t.PrimaryTag.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                t.ProjectName.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var filteredList = sourceTasks.ToList();

        // Calculate Today statistics
        var todayAll = _currentPackage.Tasks.Where(t => t.Section is "Morning" or "Afternoon" or "Evening").ToList();
        TotalTasksCount = todayAll.Count;
        CompletedTasksCount = todayAll.Count(t => t.IsCompleted);
        CompletionPercentage = TotalTasksCount > 0 ? (double)CompletedTasksCount / TotalTasksCount * 100.0 : 0.0;
        CompletionRatioText = $"{CompletedTasksCount} of {TotalTasksCount} completed ({(int)Math.Round(CompletionPercentage)}%)";

        MorningTasks.Clear();
        AfternoonTasks.Clear();
        EveningTasks.Clear();
        SectionedTasks.Clear();

        if (IsTodayView)
        {
            foreach (var t in filteredList)
            {
                if (t.Section == "Morning") MorningTasks.Add(t);
                else if (t.Section == "Afternoon") AfternoonTasks.Add(t);
                else if (t.Section == "Evening") EveningTasks.Add(t);
            }
        }
        else
        {
            foreach (var t in filteredList)
            {
                SectionedTasks.Add(t);
            }
        }
    }

    [RelayCommand]
    public void SelectNav(string category)
    {
        SelectedNavCategory = category;
        SelectedProjectId = null;
        SelectedProjectName = string.Empty;
        UpdateViewFilters();
    }

    [RelayCommand]
    public void SelectProject(PlannerProject project)
    {
        SelectedNavCategory = "Project";
        SelectedProjectId = project.Id;
        SelectedProjectName = project.Name;
        UpdateViewFilters();
    }

    [RelayCommand]
    public void SetSegmentFilter(string filter)
    {
        SegmentFilter = filter;
        UpdateViewFilters();
    }

    [RelayCommand]
    public void SetTagFilter(string tag)
    {
        if (TagFilter == tag && tag != "All")
        {
            TagFilter = "All";
        }
        else
        {
            TagFilter = tag;
        }

        foreach (var t in AvailableTags)
        {
            t.IsSelected = string.Equals(t.Name, TagFilter, StringComparison.OrdinalIgnoreCase);
        }

        UpdateViewFilters();
    }

    [RelayCommand]
    public async Task AddQuickTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle)) return;

        string section = IsTodayView ? "Morning" : (IsInboxView ? "Inbox" : (IsUpcomingView ? "Upcoming" : SelectedNavCategory));
        string? projId = IsProjectView ? SelectedProjectId : null;
        string? tag = TagFilter != "All" ? TagFilter : null;

        await _dataService.AddTaskAsync(NewTaskTitle.Trim(), section, projId, tag);
        NewTaskTitle = string.Empty;
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task ToggleTaskCompletionAsync(PlannerTask task)
    {
        await _dataService.ToggleTaskCompletionAsync(task.Id);
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task DeleteTaskAsync(PlannerTask task)
    {
        await _dataService.DeleteTaskAsync(task.Id);
        await LoadDataAsync();
    }

    [RelayCommand]
    public void ToggleExpandTask(PlannerTask task)
    {
        task.IsExpanded = !task.IsExpanded;
    }

    [RelayCommand]
    public async Task AddSubTaskAsync(PlannerTask task)
    {
        string subTitle = $"New sub-item {task.Subtasks.Count + 1}";
        await _dataService.AddSubTaskAsync(task.Id, subTitle);
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task ToggleSubTaskAsync(PlannerSubTask subTask)
    {
        if (_currentPackage == null) return;
        var parentTask = _currentPackage.Tasks.FirstOrDefault(t => t.Subtasks.Any(s => s.Id == subTask.Id));
        if (parentTask != null)
        {
            await _dataService.ToggleSubTaskAsync(parentTask.Id, subTask.Id);
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    public void OpenNewListDialog()
    {
        NewListName = string.Empty;
        NewListArea = "Work";
        IsNewListDialogOpen = true;
    }

    [RelayCommand]
    public void CloseNewListDialog()
    {
        IsNewListDialogOpen = false;
    }

    [RelayCommand]
    public async Task CreateNewListAsync()
    {
        if (string.IsNullOrWhiteSpace(NewListName)) return;

        string color = NewListArea == "Work" ? "#005FB8" : "#107C41";
        var proj = await _dataService.AddProjectAsync(NewListName.Trim(), NewListArea, color);
        IsNewListDialogOpen = false;
        await LoadDataAsync();
        SelectProject(proj);
    }

    [RelayCommand]
    public void ToggleSidebar()
    {
        IsSidebarExpanded = !IsSidebarExpanded;
    }

    [RelayCommand]
    public void OpenCreateTaskDialog()
    {
        TaskDraftTitle = string.Empty;
        TaskDraftSection = IsTodayView ? "Morning" : (IsInboxView ? "Inbox" : "Morning");
        TaskDraftDueDate = IsTodayView ? DateTimeOffset.Now : null;
        TaskDraftDueTimeText = string.Empty;
        TaskDraftDuration = string.Empty;
        TaskDraftTag = TagFilter != "All" ? TagFilter : (AvailableTags.Count > 0 ? AvailableTags[0].Name : "#Work");
        TaskDraftTagColor = AvailableTags.Count > 0 ? AvailableTags[0].ColorHex : "#005FB8";
        TaskDraftNotes = string.Empty;
        TaskDraftProjectId = IsProjectView && !string.IsNullOrEmpty(SelectedProjectId) ? SelectedProjectId : string.Empty;
        TaskDraftProjectName = IsProjectView ? SelectedProjectName : string.Empty;
        IsCreateTaskDialogOpen = true;
    }

    [RelayCommand]
    public void CloseCreateTaskDialog()
    {
        IsCreateTaskDialogOpen = false;
    }

    [RelayCommand]
    public async Task ConfirmCreateTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(TaskDraftTitle)) return;

        DateTime? date = TaskDraftDueDate?.DateTime;
        string? projColor = null;
        if (!string.IsNullOrEmpty(TaskDraftProjectId))
        {
            var p = Projects.FirstOrDefault(proj => proj.Id == TaskDraftProjectId);
            if (p != null)
            {
                projColor = p.ColorHex;
                TaskDraftProjectName = p.Name;
            }
            else
            {
                TaskDraftProjectName = string.Empty;
            }
        }
        else
        {
            TaskDraftProjectName = string.Empty;
        }

        await _dataService.AddTaskWithDetailsAsync(
            TaskDraftTitle.Trim(),
            TaskDraftSection,
            date,
            TaskDraftDueTimeText,
            TaskDraftDuration,
            TaskDraftTag?.Trim(),
            TaskDraftTagColor,
            TaskDraftNotes,
            TaskDraftProjectId,
            TaskDraftProjectName,
            projColor);

        IsCreateTaskDialogOpen = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    public void OpenEditTaskDialog(PlannerTask task)
    {
        if (task == null) return;
        EditTaskId = task.Id;
        EditTaskTitle = task.Title;
        EditTaskSection = string.IsNullOrWhiteSpace(task.Section) ? "Morning" : task.Section;
        EditTaskDueDate = task.DueDate.HasValue ? new DateTimeOffset(task.DueDate.Value) : null;
        EditTaskDueTimeText = task.DueTimeText;
        EditTaskDuration = task.DurationText;
        EditTaskTag = task.PrimaryTag;
        EditTaskTagColor = string.IsNullOrWhiteSpace(task.TagColorHex) ? "#005FB8" : task.TagColorHex;
        EditTaskNotes = task.Notes;
        EditTaskProjectId = task.ProjectId;
        EditTaskProjectName = task.ProjectName;
        IsEditTaskDialogOpen = true;
    }

    [RelayCommand]
    public void CloseEditTaskDialog()
    {
        IsEditTaskDialogOpen = false;
    }

    [RelayCommand]
    public async Task ConfirmEditTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(EditTaskTitle) || string.IsNullOrEmpty(EditTaskId)) return;

        DateTime? date = EditTaskDueDate?.DateTime;
        string? projColor = null;
        if (!string.IsNullOrEmpty(EditTaskProjectId))
        {
            var p = Projects.FirstOrDefault(proj => proj.Id == EditTaskProjectId);
            if (p != null)
            {
                projColor = p.ColorHex;
                EditTaskProjectName = p.Name;
            }
            else
            {
                EditTaskProjectName = string.Empty;
            }
        }
        else
        {
            EditTaskProjectName = string.Empty;
        }

        await _dataService.UpdateTaskAsync(
            EditTaskId,
            EditTaskTitle.Trim(),
            EditTaskSection,
            date,
            EditTaskDueTimeText,
            EditTaskDuration,
            EditTaskTag?.Trim(),
            EditTaskTagColor,
            EditTaskNotes,
            EditTaskProjectId,
            EditTaskProjectName,
            projColor);

        IsEditTaskDialogOpen = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task DeleteProjectAsync(PlannerProject project)
    {
        if (project == null) return;
        await _dataService.DeleteProjectAsync(project.Id);
        if (SelectedNavCategory == "Project" && SelectedProjectId == project.Id)
        {
            SelectNav("Inbox");
        }
        else
        {
            await LoadDataAsync();
        }
    }

    public async Task ToggleSubTaskAsync(string taskId, string subTaskId)
    {
        if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(subTaskId)) return;
        await _dataService.ToggleSubTaskAsync(taskId, subTaskId);
        await LoadDataAsync();
    }

    public async Task AddNamedSubTaskAsync(string taskId, string title)
    {
        if (string.IsNullOrWhiteSpace(taskId) || string.IsNullOrWhiteSpace(title)) return;
        await _dataService.AddSubTaskAsync(taskId, title.Trim());
        await LoadDataAsync();
    }

    public async Task DeleteSubTaskAsync(string taskId, string subTaskId)
    {
        if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(subTaskId)) return;
        await _dataService.DeleteSubTaskAsync(taskId, subTaskId);
        await LoadDataAsync();
    }

    [RelayCommand]
    public void ClearSearch()
    {
        SearchText = string.Empty;
    }

    [RelayCommand]
    public async Task UpdateTagColorAsync(PlannerTag tag)
    {
        if (tag == null) return;
        await _dataService.AddOrUpdateTagAsync(tag.Name, tag.ColorHex);
        await LoadDataAsync();
    }

    [RelayCommand]
    public void OpenSettings()
    {
        IsSettingsOpen = true;
    }

    [RelayCommand]
    public void CloseSettings()
    {
        IsSettingsOpen = false;
    }

    [RelayCommand]
    public async Task ChangeThemeAsync(int themeIndex)
    {
        SelectedThemeIndex = themeIndex;
        var theme = themeIndex switch
        {
            0 => ElementTheme.Light,
            1 => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        var current = _settingsService.CurrentSettings;
        current.Theme = theme;
        await _settingsService.SaveSettingsAsync(current);

        if (App.Window.Content is FrameworkElement root)
        {
            root.RequestedTheme = theme;
        }

        Native.Win32Interop.SetImmersiveDarkMode(App.WindowHandle, theme == ElementTheme.Dark);

        OnPropertyChanged(nameof(SelectedNavCategory));
        OnPropertyChanged(nameof(SegmentFilter));
        OnPropertyChanged(nameof(TagFilter));
        UpdateViewFilters();
        ThemeChanged?.Invoke(this, theme);
    }

    partial void OnSearchTextChanged(string value)
    {
        UpdateViewFilters();
    }
}
