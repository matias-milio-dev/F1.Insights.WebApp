namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Represents a single lap-position point for race history visualization.
/// </summary>
public sealed record DriverPositionPoint(int LapNumber, int Position);