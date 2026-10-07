using System.Text.Json.Serialization;
using Planner.Models;

namespace Planner.Services;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNameCaseInsensitive = true,
    GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(PlannerDataPackage))]
[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(PlannerTask))]
[JsonSerializable(typeof(PlannerSubTask))]
[JsonSerializable(typeof(PlannerProject))]
[JsonSerializable(typeof(CalendarHorizonEvent))]
[JsonSerializable(typeof(WeeklyDayLoad))]
[JsonSerializable(typeof(PlannerTag))]
internal partial class PlannerJsonContext : JsonSerializerContext
{
}
