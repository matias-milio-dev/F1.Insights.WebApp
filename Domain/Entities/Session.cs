namespace F1.Insights.App.Domain.Entities;

/// <summary>
/// Represents a Formula 1 weekend session option.
/// </summary>
public sealed record Session(
    int Round,
    int SessionKey,
    string SessionName,
    string SessionType,
    DateTimeOffset DateStart,
    int Year);