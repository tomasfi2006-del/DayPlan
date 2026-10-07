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
    Task<PlannerTask> AddTaskWithDetailsAsync(string title, string section, DateTime? dueDate, string dueTimeText, string? tag, string tagColor, string notes);
    Task ToggleTaskCompletionAsync(string taskId);
    Task DeleteTaskAsync(string taskId);
    Task AddSubTaskAsync(string taskId, string subTaskTitle);
    Task ToggleSubTaskAsync(string taskId, string subTaskId);
    Task<PlannerProject> AddProjectAsync(string name, string area, string colorHex);
    Task AddOrUpdateTagAsync(string name, string colorHex);
}
