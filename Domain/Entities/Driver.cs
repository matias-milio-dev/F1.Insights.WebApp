namespace F1.Insights.App.Domain.Entities;

/// <summary>
/// Represents a Formula 1 driver in a specific session context.
/// </summary>
public sealed record Driver(
    string BroadcastName,
    int DriverNumber,
    string FirstName,
    string FullName,
    string HeadshotUrl,
    string LastName,
    int MeetingKey,
    string NameAcronym,
    int SessionKey,
    string TeamColour,
    string TeamName);