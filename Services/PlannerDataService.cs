using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Planner.Models;

namespace Planner.Services;

public class PlannerDataService : IPlannerDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private PlannerDataPackage? _cachedData;
    private readonly string _storagePath;

    public PlannerDataService()
    {
        string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appDir = Path.Combine(baseDir, "PlannerSpace");
        Directory.CreateDirectory(appDir);
        _storagePath = Path.Combine(appDir, "planner_data.json");
    }

    public async Task<PlannerDataPackage> LoadDataAsync()
    {
        if (_cachedData != null)
        {
            return _cachedData;
        }

        if (File.Exists(_storagePath))
        {
            try
            {
                using var stream = File.OpenRead(_storagePath);
                var loaded = await JsonSerializer.DeserializeAsync(stream, PlannerJsonContext.Default.PlannerDataPackage);
                if (loaded != null && loaded.Tasks.Count > 0)
                {
                    _cachedData = loaded;
                    return _cachedData;
                }
            }
            catch
            {
                // Fallback to seeding initial data
            }
        }

        _cachedData = SeedInitialData();
        await SaveDataAsync(_cachedData);
        return _cachedData;
    }

    public async Task SaveDataAsync(PlannerDataPackage data)
    {
        _cachedData = data;
        string tempPath = _storagePath + ".tmp";
        string? dir = Path.GetDirectoryName(_storagePath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await JsonSerializer.SerializeAsync(fs, data, PlannerJsonContext.Default.PlannerDataPackage);
        }

        File.Move(tempPath, _storagePath, overwrite: true);
    }

    public async Task<PlannerTask> AddTaskAsync(string title, string section = "Morning", string? projectId = null, string? tag = null)
    {
        var data = await LoadDataAsync();
        var task = new PlannerTask
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = title,
            Section = section,
            PrimaryTag = tag ?? string.Empty,
            DueTimeText = section == "Morning" ? "Today" : (section == "Afternoon" ? "2:00 PM" : "Tonight"),
            IsCompleted = false
        };

        if (!string.IsNullOrEmpty(projectId))
        {
            var project = data.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null)
            {
                task.ProjectId = project.Id;
                task.ProjectName = project.Name;
                task.ProjectColor = project.ColorHex;
                project.TaskCount++;
            }
        }

        data.Tasks.Insert(0, task);
        await SaveDataAsync(data);
        return task;
    }

    public async Task ToggleTaskCompletionAsync(string taskId)
    {
        var data = await LoadDataAsync();
        var task = data.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            task.IsCompleted = !task.IsCompleted;
            task.CompletedAt = task.IsCompleted ? DateTime.Now : null;

            if (!string.IsNullOrEmpty(task.ProjectId))
            {
                var project = data.Projects.FirstOrDefault(p => p.Id == task.ProjectId);
                if (project != null)
                {
                    project.CompletedCount = data.Tasks.Count(t => t.ProjectId == project.Id && t.IsCompleted);
                }
            }

            await SaveDataAsync(data);
        }
    }

    public async Task DeleteTaskAsync(string taskId)
    {
        var data = await LoadDataAsync();
        var task = data.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            data.Tasks.Remove(task);
            if (!string.IsNullOrEmpty(task.ProjectId))
            {
                var project = data.Projects.FirstOrDefault(p => p.Id == task.ProjectId);
                if (project != null)
                {
                    project.TaskCount = data.Tasks.Count(t => t.ProjectId == project.Id);
                    project.CompletedCount = data.Tasks.Count(t => t.ProjectId == project.Id && t.IsCompleted);
                }
            }
            await SaveDataAsync(data);
        }
    }

    public async Task AddSubTaskAsync(string taskId, string subTaskTitle)
    {
        var data = await LoadDataAsync();
        var task = data.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            task.Subtasks.Add(new PlannerSubTask
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = subTaskTitle,
                IsCompleted = false
            });
            await SaveDataAsync(data);
        }
    }

    public async Task ToggleSubTaskAsync(string taskId, string subTaskId)
    {
        var data = await LoadDataAsync();
        var task = data.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            var sub = task.Subtasks.FirstOrDefault(s => s.Id == subTaskId);
            if (sub != null)
            {
                sub.IsCompleted = !sub.IsCompleted;
                await SaveDataAsync(data);
            }
        }
    }

    public async Task<PlannerProject> AddProjectAsync(string name, string area, string colorHex)
    {
        var data = await LoadDataAsync();
        var proj = new PlannerProject
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = name,
            Area = area,
            ColorHex = colorHex,
            TaskCount = 0,
            CompletedCount = 0
        };
        data.Projects.Add(proj);
        await SaveDataAsync(data);
        return proj;
    }

    public async Task<PlannerTask> AddTaskWithDetailsAsync(string title, string section, DateTime? dueDate, string dueTimeText, string? tag, string tagColor, string notes)
    {
        var data = await LoadDataAsync();
        var task = new PlannerTask
        {
            Id = Guid.NewGuid().ToString("N"),
            Title = title,
            Section = string.IsNullOrWhiteSpace(section) ? "Morning" : section,
            DueDate = dueDate,
            DueTimeText = dueTimeText ?? string.Empty,
            PrimaryTag = tag ?? string.Empty,
            TagColorHex = string.IsNullOrWhiteSpace(tagColor) ? "#005FB8" : tagColor,
            Notes = notes ?? string.Empty,
            IsCompleted = false
        };

        if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(tagColor))
        {
            var existingTag = data.Tags.FirstOrDefault(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase));
            if (existingTag == null)
            {
                data.Tags.Add(new PlannerTag { Name = tag, ColorHex = tagColor });
            }
            else
            {
                existingTag.ColorHex = tagColor;
            }
        }

        data.Tasks.Insert(0, task);
        await SaveDataAsync(data);
        return task;
    }

    public async Task AddOrUpdateTagAsync(string name, string colorHex)
    {
        var data = await LoadDataAsync();
        var existingTag = data.Tags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (existingTag != null)
        {
            existingTag.ColorHex = colorHex;
        }
        else
        {
            data.Tags.Add(new PlannerTag { Name = name, ColorHex = colorHex });
        }
        await SaveDataAsync(data);
    }

    private static PlannerDataPackage SeedInitialData()
    {
        var projects = new List<PlannerProject>();
        var tasks = new List<PlannerTask>();

        var tags = new List<PlannerTag>
        {
            new() { Name = "#Work", ColorHex = "#005FB8" },
            new() { Name = "#Personal", ColorHex = "#107C41" },
            new() { Name = "#Design", ColorHex = "#744DA9" },
            new() { Name = "#Dev", ColorHex = "#CA5010" },
            new() { Name = "#Strategy", ColorHex = "#D13438" },
            new() { Name = "#Deep Work", ColorHex = "#038387" }
        };

        var horizonEvents = new List<CalendarHorizonEvent>();

        var weeklyLoads = new List<WeeklyDayLoad>
        {
            new() { DayShort = "M", DayName = "Monday", TaskCount = 0, BarHeight = 4, IsToday = false },
            new() { DayShort = "T", DayName = "Tuesday", TaskCount = 0, BarHeight = 4, IsToday = false },
            new() { DayShort = "W", DayName = "Wednesday", TaskCount = 0, BarHeight = 4, IsToday = true },
            new() { DayShort = "T", DayName = "Thursday", TaskCount = 0, BarHeight = 4, IsToday = false },
            new() { DayShort = "F", DayName = "Friday", TaskCount = 0, BarHeight = 4, IsToday = false },
            new() { DayShort = "S", DayName = "Saturday", TaskCount = 0, BarHeight = 4, IsToday = false },
            new() { DayShort = "S", DayName = "Sunday", TaskCount = 0, BarHeight = 4, IsToday = false }
        };

        return new PlannerDataPackage
        {
            Tasks = tasks,
            Projects = projects,
            Tags = tags,
            HorizonEvents = horizonEvents,
            WeeklyLoads = weeklyLoads
        };
    }
}
