using F1.Insights.App.Infrastructure.Laps;
using F1.Insights.App.Infrastructure.Meetings;
using F1.Insights.App.Infrastructure.Pit;
using F1.Insights.App.Infrastructure.Results;
using F1.Insights.App.Infrastructure.Sessions;

namespace F1.Insights.App.Features.GrandPrixSelection;

/// <summary>
/// Coordinates meetings and sessions data for the Grand Prix selection UI.
/// </summary>
public sealed class GrandPrixSelectionService(
    IMeetingsClient meetingsClient,
    ISessionsClient sessionsClient,
    ILapsClient lapsClient,
    IPitStopClient pitStopClient,
    IResultsClient resultsClient) : IGrandPrixSelectionService
{
    private static readonly IReadOnlyDictionary<string, string> TeamColorByConstructorId =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["red_bull"] = "3671C6",
            ["ferrari"] = "E80020",
            ["mercedes"] = "27F4D2",
            ["mclaren"] = "FF8000",
            ["aston_martin"] = "229971",
            ["alpine"] = "FF87BC",
            ["williams"] = "64C4FF",
            ["rb"] = "6692FF",
            ["sauber"] = "52E252",
            ["haas"] = "B6BABD"
        };

    public async Task<IReadOnlyList<GrandPrixOption>> GetGrandPrixOptionsAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        var meetings = await meetingsClient.GetByYearAsync(year, cancellationToken);

        return [.. meetings
            .OrderBy(meeting => meeting.Round)
            .Select(static meeting => new GrandPrixOption(
                meeting.Round,
                meeting.RaceName,
                meeting.CountryName,
                meeting.Year))];
    }

    public async Task<IReadOnlyList<SessionOption>> GetSessionOptionsAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default)
    {
        var sessions = await sessionsClient.GetByYearAndRoundAsync(year, round, cancellationToken);

        return [.. sessions
            .Where(static session =>
                session.SessionType is "Practice 1" or "Practice 2" or "Practice 3" or "Qualifying" or "Race")
            .OrderBy(session => session.DateStart)
            .DistinctBy(session => session.SessionType)
            .Select(static session => new SessionOption(
                session.SessionKey,
                ToDisplayLabel(session.SessionType),
                session.SessionType,
                session.DateStart))];
    }

    public async Task<IReadOnlyList<DriverFastestLap>> GetFastestLapLeaderboardAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default)
    {
        var lapsTask = lapsClient.GetByYearAndRoundAsync(year, round, cancellationToken);
        var pitStopsTask = pitStopClient.GetByYearAndRoundAsync(year, round, cancellationToken);
        var resultsTask = resultsClient.GetByYearAndRoundAsync(year, round, cancellationToken);

        await Task.WhenAll(lapsTask, pitStopsTask, resultsTask);

        var laps = await lapsTask;
        var pitStops = await pitStopsTask;
        var results = await resultsTask;

        var resultsByDriverId = results.ToDictionary(result => result.DriverId, StringComparer.OrdinalIgnoreCase);
        var pitStopsByDriverId = pitStops.ToLookup(pit => pit.DriverId, StringComparer.OrdinalIgnoreCase);

        return [.. laps
            .Where(static lap => !string.IsNullOrWhiteSpace(lap.DriverId) && lap.LapDuration > 0)
            .GroupBy(lap => lap.DriverId, StringComparer.OrdinalIgnoreCase)
            .Select(static lapGroup => lapGroup
                .OrderBy(lap => lap.LapDuration)
                .ThenBy(lap => lap.LapNumber)
                .First())
            .OrderBy(lap => lap.LapDuration)
            .Select(lap =>
            {
                var hasResult = resultsByDriverId.TryGetValue(lap.DriverId, out var result);
                var stopCount = pitStopsByDriverId[lap.DriverId].Count();

                return new DriverFastestLap(
                    hasResult ? result!.DriverNumber ?? 0 : 0,
                    hasResult ? (string.IsNullOrWhiteSpace(result!.DriverCode) ? result.DriverName : result.DriverCode) : lap.DriverId,
                    hasResult ? result!.TeamName : "Unknown Team",
                    hasResult ? ResolveTeamColor(result!.ConstructorId) : "FFFFFF",
                    stopCount > 0 ? $"Pit Stops: {stopCount}" : "Pit Stops: 0",
                    lap.LapNumber,
                    lap.LapDuration);
            })];
    }

    private static string ResolveTeamColor(string constructorId)
        => TeamColorByConstructorId.TryGetValue(constructorId, out var color) ? color : "FFFFFF";

    private static string ToDisplayLabel(string sessionType)
        => sessionType switch
        {
            "Practice 1" => "FP1",
            "Practice 2" => "FP2",
            "Practice 3" => "FP3",
            "Qualifying" => "Qualifying",
            "Race" => "Race",
            _ => sessionType
        };
}
