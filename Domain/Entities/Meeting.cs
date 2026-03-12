namespace F1.Insights.App.Domain.Entities;

/// <summary>
/// Represents a Formula 1 meeting event.
/// </summary>
public sealed record Meeting(
    int CircuitKey,
    string CircuitInfoUrl,
    string CircuitImage,
    string CircuitShortName,
    string CircuitType,
    string CountryCode,
    string CountryFlag,
    int CountryKey,
    string CountryName,
    DateTimeOffset DateEnd,
    DateTimeOffset DateStart,
    string GmtOffset,
    string Location,
    int MeetingKey,
    string MeetingName,
    string MeetingOfficialName,
    int Year);