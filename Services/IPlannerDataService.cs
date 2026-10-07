using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Planner.Models;

namespace Planner.Services;

public class PlannerDataPackage
{
    public List<PlannerTask> Tasks { get; set; } = new();
    public List<PlannerProject> Projects { get; set; } = new();
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
        string? projectColor = null);
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
        string? projectColor);
    Task ToggleTaskCompletionAsync(string taskId);
    Task DeleteTaskAsync(string taskId);
    Task AddSubTaskAsync(string taskId, string subTaskTitle);
    Task ToggleSubTaskAsync(string taskId, string subTaskId);
    Task DeleteSubTaskAsync(string taskId, string subTaskId);
    Task<PlannerProject> AddProjectAsync(string name, string area, string colorHex);
    Task DeleteProjectAsync(string projectId);
    Task RenameProjectAsync(string projectId, string newName);
    Task AddOrUpdateTagAsync(string name, string colorHex);
}
