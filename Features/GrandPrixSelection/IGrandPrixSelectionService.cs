namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Provides selection data for Grand Prix meetings and their relevant sessions.
/// </summary>
public interface IGrandPrixSelectionService
{
    /// <summary>
    /// Retrieves meetings for a specific championship year.
    /// </summary>
    Task<IReadOnlyList<GrandPrixOption>> GetGrandPrixOptionsAsync(
        int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves selectable sessions for a meeting.
    /// </summary>
    Task<IReadOnlyList<SessionOption>> GetSessionOptionsAsync(
        int meetingKey,
        CancellationToken cancellationToken = default);
}
