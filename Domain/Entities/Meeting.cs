namespace F1.Insights.App.Domain.Entities;

/// <summary>
/// Represents a Formula 1 race event in an Ergast season.
/// </summary>
public sealed record Meeting(
    string RaceName,
    int Round,
    string CountryName,
    DateTimeOffset DateStart,
    string CircuitName,
    string Locality,
    int Year);