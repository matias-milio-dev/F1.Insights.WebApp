namespace F1.Insights.App.Domain.Entities;

/// <summary>
/// Represents a race lap timing for a driver.
/// </summary>
public sealed record Lap(
    string DriverId,
    int? Position,
    double LapDuration,
    int LapNumber,
    int Round,
    int Year);
