namespace F1.Insights.App.Domain.Entities;

/// <summary>
/// Represents a tire stint for a driver during a session.
/// </summary>
public sealed record Stint(
    string Compound,
    int DriverNumber,
    int? LapEnd,
    int LapStart,
    int MeetingKey,
    int SessionKey,
    int StintNumber,
    int? TyreAgeAtStart);