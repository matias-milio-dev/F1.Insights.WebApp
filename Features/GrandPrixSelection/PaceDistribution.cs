namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Represents box-plot statistics for a driver's lap-time consistency.
/// </summary>
public sealed record PaceDistribution(
    string DriverId,
    string DriverLabel,
    string TeamColorHex,
    double Min,
    double Q1,
    double Median,
    double Q3,
    double Max,
    int OutlierCount);