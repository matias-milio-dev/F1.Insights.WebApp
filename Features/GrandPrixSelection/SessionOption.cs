namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Represents a session option displayed to the user.
/// </summary>
public sealed record SessionOption(
    int SessionKey,
    string Label,
    string SessionType,
    DateTimeOffset DateStart);