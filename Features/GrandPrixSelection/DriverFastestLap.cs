namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Represents a driver's fastest lap in a selected session.
/// </summary>
public sealed record DriverFastestLap(
    int DriverNumber,
    string DriverName,
    string TeamName,
    string TeamColourHex,
    string Compound,
    int LapNumber,
    double LapTimeSeconds);