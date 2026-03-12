namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Represents a Grand Prix option displayed to the user.
/// </summary>
public sealed record GrandPrixOption(
    int MeetingKey,
    string Label,
    string CountryName,
    int Year);