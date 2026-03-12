using F1.Insights.App.Infrastructure.Meetings;
using F1.Insights.App.Infrastructure.Sessions;

namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Coordinates meetings and sessions data for the Grand Prix selection UI.
/// </summary>
public sealed class GrandPrixSelectionService(
    IMeetingsClient meetingsClient,
    ISessionsClient sessionsClient) : IGrandPrixSelectionService
{
    public async Task<IReadOnlyList<GrandPrixOption>> GetGrandPrixOptionsAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        var meetings = await meetingsClient.GetByYearAsync(year, cancellationToken);

        return [.. meetings
            .OrderBy(meeting => meeting.DateStart)
            .Select(static meeting => new GrandPrixOption(
                meeting.MeetingKey,
                meeting.MeetingName,
                meeting.CountryName,
                meeting.Year))];
    }

    public async Task<IReadOnlyList<SessionOption>> GetSessionOptionsAsync(
        int meetingKey,
        CancellationToken cancellationToken = default)
    {
        var sessions = await sessionsClient.GetByMeetingKeyAsync(meetingKey, cancellationToken);

        return [.. sessions
            .Where(static session =>
                session.SessionType is "Practice 1" or "Qualifying" or "Race")
            .OrderBy(session => session.DateStart)
            .DistinctBy(session => session.SessionType)
            .Select(static session => new SessionOption(
                session.SessionKey,
                ToDisplayLabel(session.SessionType),
                session.SessionType,
                session.DateStart))];
    }

    private static string ToDisplayLabel(string sessionType)
        => sessionType switch
        {
            "Practice 1" => "FP1",
            "Qualifying" => "Qualifying",
            "Race" => "Race",
            _ => sessionType
        };
}
