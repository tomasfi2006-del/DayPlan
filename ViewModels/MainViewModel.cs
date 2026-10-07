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
    public partial bool HasAnyProjects { get; set; }

    [ObservableProperty]
    public partial string ProjectPlaceholderText { get; set; } = "Non Existent";

    [ObservableProperty]
    public partial bool HasAreas { get; set; }

    [ObservableProperty]
    public partial string DraftTagInputText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditTagInputText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string DraftSubtaskInputText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EditSubtaskInputText { get; set; } = string.Empty;

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

    [ObservableProperty]
    public partial string CurrentStoragePath { get; set; } = string.Empty;

    public ObservableCollection<PlannerTag> AvailableTags { get; } = new();
    public ObservableCollection<PlannerTag> FilteredTags { get; } = new();

    [ObservableProperty]
    public partial string TagDropdownSearchText { get; set; } = string.Empty;

    public string TagFilterButtonText => string.IsNullOrEmpty(TagFilter) || TagFilter == "All" ? "All Tags" : TagFilter;
    public bool HasActiveTagFilter => !string.IsNullOrEmpty(TagFilter) && TagFilter != "All";

    public bool IsDarkTheme => SelectedThemeIndex switch
    {
        0 => false,
        1 => true,
        _ => Application.Current.RequestedTheme == ApplicationTheme.Dark
    };

    public ElementTheme CurrentTheme => SelectedThemeIndex switch
    {
        0 => ElementTheme.Light,
        1 => ElementTheme.Dark,
        _ => ElementTheme.Default
    };

    public ObservableCollection<PlannerTask> MorningTasks { get; } = new();
    public ObservableCollection<PlannerTask> AfternoonTasks { get; } = new();
    public ObservableCollection<PlannerTask> EveningTasks { get; } = new();
    public ObservableCollection<PlannerTask> SectionedTasks { get; } = new();

    public ObservableCollection<PlannerProject> Projects { get; } = new();
    public ObservableCollection<PlannerProject> WorkProjects { get; } = new();
    public ObservableCollection<PlannerProject> PersonalProjects { get; } = new();
    public ObservableCollection<ProjectAreaGroup> AreaGroups { get; } = new();

    public ObservableCollection<PlannerTag> DraftTaskTags { get; } = new();
    public ObservableCollection<PlannerTag> EditTaskTags { get; } = new();
    public ObservableCollection<PlannerSubTask> DraftSubtasks { get; } = new();
    public ObservableCollection<PlannerSubTask> EditSubtasks { get; } = new();

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
        CurrentStoragePath = _settingsService.CurrentStorageDirectory;

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
        HasAnyProjects = Projects.Count > 0;
        ProjectPlaceholderText = HasAnyProjects ? "None" : "Non Existent";

        AreaGroups.Clear();
        var areasList = (_currentPackage.Areas != null)
            ? new System.Collections.Generic.List<string>(_currentPackage.Areas)
            : new System.Collections.Generic.List<string>();

        foreach (var p in _currentPackage.Projects)
        {
            if (!string.IsNullOrWhiteSpace(p.Area) && !areasList.Contains(p.Area, StringComparer.OrdinalIgnoreCase))
            {
                areasList.Add(p.Area);
            }
        }

        foreach (var area in areasList)
        {
            var group = new ProjectAreaGroup { AreaName = area };
            foreach (var p in _currentPackage.Projects.Where(proj => string.Equals(proj.Area, area, StringComparison.OrdinalIgnoreCase)))
            {
                group.Projects.Add(p);
            }
            AreaGroups.Add(group);
        }

        HasAreas = AreaGroups.Count > 0;

        // Ensure default tags #Work, #Personal, #Focus exist if tag list is empty
        if (_currentPackage.Tags == null || _currentPackage.Tags.Count == 0)
        {
            _currentPackage.Tags = new System.Collections.Generic.List<PlannerTag>
            {
                new() { Name = "#Work", ColorHex = "#005FB8" },
                new() { Name = "#Personal", ColorHex = "#107C41" },
                new() { Name = "#Focus", ColorHex = "#0078D4" }
            };
        }

        AvailableTags.Clear();
        FilteredTags.Clear();
        foreach (var tag in _currentPackage.Tags)
        {
            tag.IsSelected = string.Equals(tag.Name, TagFilter, StringComparison.OrdinalIgnoreCase);
            AvailableTags.Add(tag);
            FilteredTags.Add(tag);
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
            sourceTasks = sourceTasks.Where(t =>
                t.PrimaryTag.Equals(TagFilter, StringComparison.OrdinalIgnoreCase) ||
                t.Tags.Any(tg => tg.Name.Equals(TagFilter, StringComparison.OrdinalIgnoreCase)));
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

        OnPropertyChanged(nameof(TagFilterButtonText));
        OnPropertyChanged(nameof(HasActiveTagFilter));
        UpdateViewFilters();
    }

    public async Task AddNewFilterTagAsync(string name, string colorHex)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        string formatted = name.Trim();
        if (!formatted.StartsWith('#')) formatted = "#" + formatted;
        await _dataService.AddOrUpdateTagAsync(formatted, string.IsNullOrWhiteSpace(colorHex) ? "#005FB8" : colorHex);
        await LoadDataAsync();
    }

    public async Task UpdateTagColorAsync(string name, string colorHex)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        await _dataService.AddOrUpdateTagAsync(name.Trim(), colorHex);
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task DeleteTagFilterAsync(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName)) return;
        await _dataService.DeleteTagAsync(tagName.Trim());
        if (string.Equals(TagFilter, tagName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            TagFilter = "All";
        }
        await LoadDataAsync();
    }

    [RelayCommand]
    public void AddTagToDraft(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName)) return;
        string formatted = tagName.Trim();
        if (!formatted.StartsWith('#')) formatted = "#" + formatted;
        if (!DraftTaskTags.Any(t => t.Name.Equals(formatted, StringComparison.OrdinalIgnoreCase)))
        {
            var existing = AvailableTags.FirstOrDefault(t => t.Name.Equals(formatted, StringComparison.OrdinalIgnoreCase));
            DraftTaskTags.Add(new PlannerTag
            {
                Name = formatted,
                ColorHex = existing?.ColorHex ?? "#005FB8"
            });
        }
    }

    [RelayCommand]
    public void AddTagToEdit(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName)) return;
        string formatted = tagName.Trim();
        if (!formatted.StartsWith('#')) formatted = "#" + formatted;
        if (!EditTaskTags.Any(t => t.Name.Equals(formatted, StringComparison.OrdinalIgnoreCase)))
        {
            var existing = AvailableTags.FirstOrDefault(t => t.Name.Equals(formatted, StringComparison.OrdinalIgnoreCase));
            EditTaskTags.Add(new PlannerTag
            {
                Name = formatted,
                ColorHex = existing?.ColorHex ?? "#005FB8"
            });
        }
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

        DraftTaskTags.Clear();
        DraftSubtasks.Clear();
        DraftTagInputText = string.Empty;
        DraftSubtaskInputText = string.Empty;

        if (TagFilter != "All" && !string.IsNullOrWhiteSpace(TagFilter))
        {
            var existing = AvailableTags.FirstOrDefault(t => t.Name.Equals(TagFilter, StringComparison.OrdinalIgnoreCase));
            DraftTaskTags.Add(new PlannerTag
            {
                Name = TagFilter,
                ColorHex = existing?.ColorHex ?? "#005FB8"
            });
        }

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

        string? primaryTag = DraftTaskTags.Count > 0 ? DraftTaskTags[0].Name : (!string.IsNullOrWhiteSpace(TaskDraftTag) ? TaskDraftTag.Trim() : null);
        string primaryTagColor = DraftTaskTags.Count > 0 ? DraftTaskTags[0].ColorHex : TaskDraftTagColor;

        await _dataService.AddTaskWithDetailsAsync(
            TaskDraftTitle.Trim(),
            TaskDraftSection,
            date,
            TaskDraftDueTimeText,
            TaskDraftDuration,
            primaryTag,
            primaryTagColor,
            TaskDraftNotes,
            TaskDraftProjectId,
            TaskDraftProjectName,
            projColor,
            DraftTaskTags,
            DraftSubtasks);

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

        EditTaskTags.Clear();
        EditSubtasks.Clear();
        EditTagInputText = string.Empty;
        EditSubtaskInputText = string.Empty;

        if (task.Tags != null && task.Tags.Count > 0)
        {
            foreach (var tg in task.Tags)
            {
                EditTaskTags.Add(new PlannerTag { Id = tg.Id, Name = tg.Name, ColorHex = tg.ColorHex });
            }
        }
        else if (!string.IsNullOrWhiteSpace(task.PrimaryTag))
        {
            EditTaskTags.Add(new PlannerTag { Name = task.PrimaryTag, ColorHex = task.TagColorHex });
        }

        if (task.Subtasks != null && task.Subtasks.Count > 0)
        {
            foreach (var st in task.Subtasks)
            {
                EditSubtasks.Add(new PlannerSubTask { Id = st.Id, Title = st.Title, IsCompleted = st.IsCompleted });
            }
        }

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

        string? primaryTag = EditTaskTags.Count > 0 ? EditTaskTags[0].Name : (!string.IsNullOrWhiteSpace(EditTaskTag) ? EditTaskTag.Trim() : null);
        string primaryTagColor = EditTaskTags.Count > 0 ? EditTaskTags[0].ColorHex : EditTaskTagColor;

        await _dataService.UpdateTaskAsync(
            EditTaskId,
            EditTaskTitle.Trim(),
            EditTaskSection,
            date,
            EditTaskDueTimeText,
            EditTaskDuration,
            primaryTag,
            primaryTagColor,
            EditTaskNotes,
            EditTaskProjectId,
            EditTaskProjectName,
            projColor,
            EditTaskTags,
            EditSubtasks);

        IsEditTaskDialogOpen = false;
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task CreateAreaAsync(string areaName)
    {
        if (string.IsNullOrWhiteSpace(areaName)) return;
        await _dataService.AddAreaAsync(areaName.Trim());
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task DeleteAreaAsync(string areaName)
    {
        if (string.IsNullOrWhiteSpace(areaName)) return;
        await _dataService.DeleteAreaAsync(areaName.Trim());
        await LoadDataAsync();
    }

    public async Task CreateListInAreaAsync(string areaName, string listName)
    {
        if (string.IsNullOrWhiteSpace(listName)) return;
        string safeArea = string.IsNullOrWhiteSpace(areaName) ? "Work" : areaName.Trim();
        var proj = await _dataService.AddProjectAsync(listName.Trim(), safeArea, "#005FB8");
        await LoadDataAsync();
        SelectProject(proj);
    }

    public async Task<PlannerProject?> CreateProjectQuickAsync(string name, string area = "Work")
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var proj = await _dataService.AddProjectAsync(name.Trim(), string.IsNullOrWhiteSpace(area) ? "Work" : area.Trim(), "#005FB8");
        await LoadDataAsync();
        return proj;
    }

    [RelayCommand]
    public void AddDraftTag(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName)) return;
        string tag = tagName.Trim();
        if (!tag.StartsWith("#")) tag = "#" + tag;
        if (!DraftTaskTags.Any(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
        {
            var existing = AvailableTags.FirstOrDefault(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase));
            string color = existing?.ColorHex ?? "#005FB8";
            DraftTaskTags.Add(new PlannerTag { Name = tag, ColorHex = color });
        }
        DraftTagInputText = string.Empty;
    }

    [RelayCommand]
    public void RemoveDraftTag(PlannerTag tag)
    {
        if (tag != null) DraftTaskTags.Remove(tag);
    }

    public void UpdateDraftTagColor(PlannerTag tag, string colorHex)
    {
        if (tag == null || string.IsNullOrWhiteSpace(colorHex)) return;
        tag.ColorHex = colorHex;
        _ = _dataService.AddOrUpdateTagAsync(tag.Name, colorHex);
    }

    [RelayCommand]
    public void AddDraftSubtask(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return;
        DraftSubtasks.Add(new PlannerSubTask
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = title.Trim(),
            IsCompleted = false
        });
        DraftSubtaskInputText = string.Empty;
    }

    [RelayCommand]
    public void RemoveDraftSubtask(PlannerSubTask subtask)
    {
        if (subtask != null) DraftSubtasks.Remove(subtask);
    }

    [RelayCommand]
    public void ToggleDraftSubtask(PlannerSubTask subtask)
    {
        if (subtask != null) subtask.IsCompleted = !subtask.IsCompleted;
    }

    [RelayCommand]
    public void AddEditTag(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName)) return;
        string tag = tagName.Trim();
        if (!tag.StartsWith("#")) tag = "#" + tag;
        if (!EditTaskTags.Any(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
        {
            var existing = AvailableTags.FirstOrDefault(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase));
            string color = existing?.ColorHex ?? "#005FB8";
            EditTaskTags.Add(new PlannerTag { Name = tag, ColorHex = color });
        }
        EditTagInputText = string.Empty;
    }

    [RelayCommand]
    public void RemoveEditTag(PlannerTag tag)
    {
        if (tag != null) EditTaskTags.Remove(tag);
    }

    public void UpdateEditTagColor(PlannerTag tag, string colorHex)
    {
        if (tag == null || string.IsNullOrWhiteSpace(colorHex)) return;
        tag.ColorHex = colorHex;
        _ = _dataService.AddOrUpdateTagAsync(tag.Name, colorHex);
    }

    [RelayCommand]
    public void AddEditSubtask(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return;
        EditSubtasks.Add(new PlannerSubTask
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = title.Trim(),
            IsCompleted = false
        });
        EditSubtaskInputText = string.Empty;
    }

    [RelayCommand]
    public void RemoveEditSubtask(PlannerSubTask subtask)
    {
        if (subtask != null) EditSubtasks.Remove(subtask);
    }

    [RelayCommand]
    public void ToggleEditSubtask(PlannerSubTask subtask)
    {
        if (subtask != null) subtask.IsCompleted = !subtask.IsCompleted;
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
        OnPropertyChanged(nameof(IsDarkTheme));
        OnPropertyChanged(nameof(CurrentTheme));
        UpdateViewFilters();
        ThemeChanged?.Invoke(this, theme);
    }

    public void FilterDropdownTags(string query)
    {
        FilteredTags.Clear();
        if (string.IsNullOrWhiteSpace(query))
        {
            foreach (var tag in AvailableTags) FilteredTags.Add(tag);
        }
        else
        {
            foreach (var tag in AvailableTags.Where(t => t.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))
            {
                FilteredTags.Add(tag);
            }
        }
    }

    [RelayCommand]
    public void SelectTagFilter(string tag)
    {
        SetTagFilter(tag);
    }

    [RelayCommand]
    public void ClearTagFilter()
    {
        SetTagFilter("All");
    }

    [RelayCommand]
    public async Task ChangeStorageLocationAsync(string newFolder)
    {
        if (string.IsNullOrWhiteSpace(newFolder)) return;
        bool changed = await _settingsService.ChangeStorageDirectoryAsync(newFolder);
        if (changed)
        {
            CurrentStoragePath = _settingsService.CurrentStorageDirectory;
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    public async Task ResetStorageLocationAsync()
    {
        await _settingsService.ResetStorageDirectoryAsync();
        CurrentStoragePath = _settingsService.CurrentStorageDirectory;
        await LoadDataAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        UpdateViewFilters();
    }
}
