using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Planner.Models;

public partial class PlannerSubTask : ObservableObject
{
    [ObservableProperty]
    public partial string Id { get; set; } = Guid.NewGuid().ToString("N");

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsCompleted { get; set; }
}

public partial class PlannerTask : ObservableObject
{
    [ObservableProperty]
    public partial string Id { get; set; } = Guid.NewGuid().ToString("N");

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Notes { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Section { get; set; } = "Morning"; // Morning, Afternoon, Evening, Inbox, Upcoming, Anytime, Someday

    [ObservableProperty]
    public partial string ProjectId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ProjectName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ProjectColor { get; set; } = "#005FB8";

    [ObservableProperty]
    public partial string DueTimeText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string DurationText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsCompleted { get; set; }

    [ObservableProperty]
    public partial bool IsCalendarSynced { get; set; }

    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    [ObservableProperty]
    public partial DateTime? DueDate { get; set; }

    [ObservableProperty]
    public partial DateTime? CompletedAt { get; set; }

    [ObservableProperty]
    public partial string PrimaryTag { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TagColorHex { get; set; } = "#005FB8";

    [ObservableProperty]
    public partial ObservableCollection<PlannerTag> Tags { get; set; } = new();

    [ObservableProperty]
    public partial ObservableCollection<PlannerSubTask> Subtasks { get; set; } = new();

    public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);
    public bool HasProject => !string.IsNullOrWhiteSpace(ProjectName);
    public bool HasTag => !string.IsNullOrWhiteSpace(PrimaryTag) || (Tags != null && Tags.Count > 0);
    public bool HasDueTime => !string.IsNullOrWhiteSpace(DueTimeText);
    public bool HasDuration => !string.IsNullOrWhiteSpace(DurationText);
    public bool HasSubtasks => Subtasks != null && Subtasks.Count > 0;

    partial void OnNotesChanged(string value) => OnPropertyChanged(nameof(HasNotes));
    partial void OnProjectNameChanged(string value) => OnPropertyChanged(nameof(HasProject));
    partial void OnPrimaryTagChanged(string value) => OnPropertyChanged(nameof(HasTag));
    partial void OnDueTimeTextChanged(string value) => OnPropertyChanged(nameof(HasDueTime));
    partial void OnDurationTextChanged(string value) => OnPropertyChanged(nameof(HasDuration));
    partial void OnSubtasksChanged(ObservableCollection<PlannerSubTask> value) => OnPropertyChanged(nameof(HasSubtasks));
}

public partial class PlannerProject : ObservableObject
{
    [ObservableProperty]
    public partial string Id { get; set; } = Guid.NewGuid().ToString("N");

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Area { get; set; } = "Work"; // Work or Personal

    [ObservableProperty]
    public partial string ColorHex { get; set; } = "#005FB8";

    [ObservableProperty]
    public partial int TaskCount { get; set; }

    [ObservableProperty]
    public partial int CompletedCount { get; set; }

    public double ProgressPercentage => TaskCount > 0 ? (double)CompletedCount / TaskCount * 100.0 : 0.0;
}

public partial class CalendarHorizonEvent : ObservableObject
{
    [ObservableProperty]
    public partial string Id { get; set; } = Guid.NewGuid().ToString("N");

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TimeRange { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Location { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string AccentColor { get; set; } = "#005FB8";

    [ObservableProperty]
    public partial bool IsActive { get; set; }
}

public partial class WeeklyDayLoad : ObservableObject
{
    [ObservableProperty]
    public partial string DayShort { get; set; } = string.Empty; // M, T, W, T, F, S, S

    [ObservableProperty]
    public partial string DayName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int TaskCount { get; set; }

    [ObservableProperty]
    public partial bool IsToday { get; set; }

    [ObservableProperty]
    public partial double BarHeight { get; set; }
}

public partial class PlannerTag : ObservableObject
{
    [ObservableProperty]
    public partial string Id { get; set; } = Guid.NewGuid().ToString("N");

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ColorHex { get; set; } = "#005FB8";

    [ObservableProperty]
    [property: System.Text.Json.Serialization.JsonIgnore]
    public partial bool IsSelected { get; set; }
}

public partial class ProjectAreaGroup : ObservableObject
{
    [ObservableProperty]
    public partial string AreaName { get; set; } = string.Empty;

    public ObservableCollection<PlannerProject> Projects { get; } = new();
}

