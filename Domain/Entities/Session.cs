namespace F1.Insights.App.Domain.Entities;

public sealed record Session(
    int CircuitKey,
    string CircuitShortName,
    string CountryCode,
    int CountryKey,
    string CountryName,
    DateTimeOffset DateEnd,
    DateTimeOffset DateStart,
    string GmtOffset,
    string Location,
    int MeetingKey,
    int SessionKey,
    string SessionName,
    string SessionType,
    int Year);