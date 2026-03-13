namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Represents duel metrics for a single driver in the selected race.
/// </summary>
public sealed record DuelDriverStats(
    string DriverId,
    string DriverLabel,
    string TeamName,
    string TeamColorHex,
    int? FinishPosition,
    double? FastestLapSeconds,
    double? AverageLapSeconds,
    int PitStopCount);