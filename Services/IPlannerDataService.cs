using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Planner.Models;

namespace Planner.Services;

public class PlannerDataPackage
{
    public List<PlannerTask> Tasks { get; set; } = new();
    public List<PlannerProject> Projects { get; set; } = new();
    public List<string> Areas { get; set; } = new() { "Work", "Personal" };
    public List<PlannerTag> Tags { get; set; } = new();
    public List<CalendarHorizonEvent> HorizonEvents { get; set; } = new();
    public List<WeeklyDayLoad> WeeklyLoads { get; set; } = new();
}

public interface IPlannerDataService
{
    Task<PlannerDataPackage> LoadDataAsync();
    Task SaveDataAsync(PlannerDataPackage data);
    Task<PlannerTask> AddTaskAsync(string title, string section = "Morning", string? projectId = null, string? tag = null);
    Task<PlannerTask> AddTaskWithDetailsAsync(
        string title,
        string section,
        DateTime? dueDate,
        string dueTimeText,
        string durationText,
        string? tag,
        string tagColor,
        string notes,
        string? projectId = null,
        string? projectName = null,
        string? projectColor = null,
        IEnumerable<PlannerTag>? tags = null,
        IEnumerable<PlannerSubTask>? subtasks = null);
    Task<PlannerTask?> UpdateTaskAsync(
        string taskId,
        string title,
        string section,
        DateTime? dueDate,
        string dueTimeText,
        string durationText,
        string? tag,
        string tagColor,
        string notes,
        string? projectId,
        string? projectName,
        string? projectColor,
        IEnumerable<PlannerTag>? tags = null,
        IEnumerable<PlannerSubTask>? subtasks = null);
    Task ToggleTaskCompletionAsync(string taskId);
    Task DeleteTaskAsync(string taskId);
    Task AddSubTaskAsync(string taskId, string subTaskTitle);
    Task ToggleSubTaskAsync(string taskId, string subTaskId);
    Task DeleteSubTaskAsync(string taskId, string subTaskId);
    Task<PlannerProject> AddProjectAsync(string name, string area, string colorHex);
    Task DeleteProjectAsync(string projectId);
    Task RenameProjectAsync(string projectId, string newName);
    Task AddAreaAsync(string areaName);
    Task DeleteAreaAsync(string areaName);
    Task AddOrUpdateTagAsync(string name, string colorHex);
    Task DeleteTagAsync(string tagName);
}
