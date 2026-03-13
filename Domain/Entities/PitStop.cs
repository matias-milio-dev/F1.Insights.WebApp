namespace F1.Insights.App.Domain.Entities;

/// <summary>
/// Represents a pit stop event for a driver.
/// </summary>
public sealed record PitStop(
    string DriverId,
    int Stop,
    int LapNumber,
    int Round,
    int Year,
    TimeSpan? Duration);
