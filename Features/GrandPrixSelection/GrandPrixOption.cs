namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Represents a Grand Prix option displayed to the user.
/// </summary>
public sealed record GrandPrixOption(
    int Round,
    string Label,
    string CountryName,
    string CountryFlag,
    int Year);