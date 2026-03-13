namespace F1.Insights.App.Domain.Entities;

/// <summary>
/// Represents a driver's race result for a specific round.
/// </summary>
public sealed record RaceResult(
    string DriverId,
    string DriverCode,
    string DriverName,
    int? DriverNumber,
    string ConstructorId,
    string TeamName,
    int Position,
    int? FastestLapNumber,
    double? FastestLapTimeSeconds,
    int Round,
    int Year);