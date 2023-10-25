using ShiftsLogger.UI.Controllers;
using ShiftsLogger.UI.Exceptions;
using ShiftsLogger.UI.Models.DTOs;
using Spectre.Console;

namespace ShiftsLogger.UI.Services;

public static class ShiftService
{
    public static void ShowShifts()
    {
        try
        {
            var shifts = ShiftController.GetShifts();

            if (shifts.Any())
            {
                var shiftsForView = shifts.Select(shift =>
                    new ShiftViewDto
                    {
                        WorkerName = shift.WorkerName,
                        StartedAt = shift.StartedAt,
                        FinishedAt = shift.FinishedAt,
                        Duration = shift.Duration
                    }).ToList();

                Visualization.ShowShiftsTable(shiftsForView);
            }
            else
            {
                Console.WriteLine("No logged shifts.");
            }
        }
        catch (ApiException ex)
        {
            var messageParts = ex.Message.Split(":");
            AnsiConsole.MarkupLineInterpolated($"[red]{messageParts[0]}[/]{messageParts[1]}");
        }
    }

    public static void ShowShiftDetails()
    {
        try
        {
            var shiftId = GetShiftIdInput();

            if (shiftId != null)
            {
                var shift = ShiftController.GetShiftById((long)shiftId);
                var shiftForView = new ShiftViewDto
                {
                    WorkerName = shift!.WorkerName,
                    StartedAt = shift.StartedAt,
                    FinishedAt = shift.FinishedAt,
                    Duration = shift.Duration
                };

                Visualization.ShowShiftDetails(shiftForView);
            }
            else
            {
                Console.WriteLine("No logged shifts.");
            }
        }
        catch (ApiException ex)
        {
            var messageParts = ex.Message.Split(":");
            AnsiConsole.MarkupLineInterpolated($"[red]{messageParts[0]}[/]{messageParts[1]}");
        }
    }

    private static long? GetShiftIdInput()
    {
        var shifts = ShiftController.GetShifts();
        if (!shifts.Any()) return null;

        var shiftsOptions = shifts.Select(shift =>
            new ShiftViewDto
            {
                WorkerName = shift.WorkerName,
                StartedAt = shift.StartedAt,
                FinishedAt = shift.FinishedAt,
                Duration = shift.Duration
            }).ToList();

        var selected = AnsiConsole.Prompt(new SelectionPrompt<ShiftViewDto>()
            .Title("Choose shift")
            .AddChoices(shiftsOptions));
        var id = shifts.Find(shift =>
            shift.WorkerName == selected.WorkerName &&
            shift.StartedAt == selected.StartedAt &&
            shift.FinishedAt == selected.FinishedAt)!.Id;

        return id;
    }
}