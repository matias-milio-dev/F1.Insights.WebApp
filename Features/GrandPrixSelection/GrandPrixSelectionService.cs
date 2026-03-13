using F1.Insights.App.Infrastructure.Laps;
using F1.Insights.App.Infrastructure.Meetings;
using F1.Insights.App.Infrastructure.Pit;
using F1.Insights.App.Infrastructure.Results;
using F1.Insights.App.Infrastructure.Sessions;
using System.Globalization;

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

    private static readonly IReadOnlyDictionary<string, string> CountryFlagByCountryName =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Australia"] = "🇦🇺",
            ["Bahrain"] = "🇧🇭",
            ["Saudi Arabia"] = "🇸🇦",
            ["Japan"] = "🇯🇵",
            ["China"] = "🇨🇳",
            ["United States"] = "🇺🇸",
            ["Italy"] = "🇮🇹",
            ["Monaco"] = "🇲🇨",
            ["Canada"] = "🇨🇦",
            ["Spain"] = "🇪🇸",
            ["Austria"] = "🇦🇹",
            ["United Kingdom"] = "🇬🇧",
            ["Hungary"] = "🇭🇺",
            ["Belgium"] = "🇧🇪",
            ["Netherlands"] = "🇳🇱",
            ["Azerbaijan"] = "🇦🇿",
            ["Singapore"] = "🇸🇬",
            ["Mexico"] = "🇲🇽",
            ["Brazil"] = "🇧🇷",
            ["Qatar"] = "🇶🇦",
            ["United Arab Emirates"] = "🇦🇪",
            ["France"] = "🇫🇷",
            ["Germany"] = "🇩🇪"
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
                ResolveCountryFlag(meeting.CountryName),
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
        var context = await GetRaceContextAsync(year, round, cancellationToken);

        return [.. context.Laps
            .Where(static lap => !string.IsNullOrWhiteSpace(lap.DriverId) && lap.LapDuration > 0)
            .GroupBy(lap => lap.DriverId, StringComparer.OrdinalIgnoreCase)
            .Select(static lapGroup => lapGroup
                .OrderBy(lap => lap.LapDuration)
                .ThenBy(lap => lap.LapNumber)
                .First())
            .OrderBy(lap => lap.LapDuration)
            .Select(lap =>
            {
                var hasResult = context.ResultsByDriverId.TryGetValue(lap.DriverId, out var result);
                var stopCount = context.PitStopsByDriverId[lap.DriverId].Count();

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

    public async Task<IReadOnlyList<DuelDriverStats>> GetDuelCandidatesAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default)
    {
        var context = await GetRaceContextAsync(year, round, cancellationToken);

        return [.. context.Results
            .OrderBy(result => result.Position)
            .Select(result =>
            {
                var laps = context.LapsByDriverId[result.DriverId]
                    .Where(lap => lap.LapDuration > 0)
                    .Select(lap => lap.LapDuration)
                    .ToArray();

                return new DuelDriverStats(
                    result.DriverId,
                    string.IsNullOrWhiteSpace(result.DriverCode) ? result.DriverName : result.DriverCode,
                    result.TeamName,
                    ResolveTeamColor(result.ConstructorId),
                    result.Position,
                    laps.Length > 0 ? laps.Min() : null,
                    laps.Length > 0 ? laps.Average() : null,
                    context.PitStopsByDriverId[result.DriverId].Count());
            })];
    }

    public async Task<IReadOnlyList<DriverPositionPoint>> GetRaceHistoryAsync(
        int year,
        int round,
        string driverId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(driverId))
        {
            return [];
        }

        var context = await GetRaceContextAsync(year, round, cancellationToken);

        return [.. context.LapsByDriverId[driverId]
            .Where(lap => lap.Position is > 0)
            .OrderBy(lap => lap.LapNumber)
            .Select(lap => new DriverPositionPoint(lap.LapNumber, lap.Position!.Value))];
    }

    public async Task<PaceDistribution?> GetPaceDistributionAsync(
        int year,
        int round,
        string driverId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(driverId))
        {
            return null;
        }

        var context = await GetRaceContextAsync(year, round, cancellationToken);

        if (!context.ResultsByDriverId.TryGetValue(driverId, out var result))
        {
            return null;
        }

        var lapTimes = context.LapsByDriverId[driverId]
            .Where(lap => lap.LapDuration > 0)
            .Select(lap => lap.LapDuration)
            .OrderBy(lap => lap)
            .ToArray();

        if (lapTimes.Length == 0)
        {
            return null;
        }

        var q1 = GetPercentile(lapTimes, 0.25);
        var median = GetPercentile(lapTimes, 0.5);
        var q3 = GetPercentile(lapTimes, 0.75);

        var iqr = q3 - q1;
        var lowerFence = q1 - (1.5 * iqr);
        var upperFence = q3 + (1.5 * iqr);

        var outlierCount = lapTimes.Count(value => value < lowerFence || value > upperFence);

        return new PaceDistribution(
            result.DriverId,
            string.IsNullOrWhiteSpace(result.DriverCode) ? result.DriverName : result.DriverCode,
            ResolveTeamColor(result.ConstructorId),
            lapTimes.Min(),
            q1,
            median,
            q3,
            lapTimes.Max(),
            outlierCount);
    }

    private async Task<RaceContext> GetRaceContextAsync(int year, int round, CancellationToken cancellationToken)
    {
        var lapsTask = lapsClient.GetByYearAndRoundAsync(year, round, cancellationToken);
        var pitStopsTask = pitStopClient.GetByYearAndRoundAsync(year, round, cancellationToken);
        var resultsTask = resultsClient.GetByYearAndRoundAsync(year, round, cancellationToken);

        await Task.WhenAll(lapsTask, pitStopsTask, resultsTask);

        var laps = await lapsTask;
        var pitStops = await pitStopsTask;
        var results = await resultsTask;

        return new RaceContext(
            laps,
            laps.ToLookup(lap => lap.DriverId, StringComparer.OrdinalIgnoreCase),
            results,
            results.ToDictionary(result => result.DriverId, StringComparer.OrdinalIgnoreCase),
            pitStops.ToLookup(pit => pit.DriverId, StringComparer.OrdinalIgnoreCase));
    }

    private static double GetPercentile(IReadOnlyList<double> sortedValues, double percentile)
    {
        if (sortedValues.Count == 1)
        {
            return sortedValues[0];
        }

        var position = (sortedValues.Count - 1) * percentile;
        var lowerIndex = (int)Math.Floor(position);
        var upperIndex = (int)Math.Ceiling(position);

        if (lowerIndex == upperIndex)
        {
            return sortedValues[lowerIndex];
        }

        var weight = position - lowerIndex;
        return sortedValues[lowerIndex] + ((sortedValues[upperIndex] - sortedValues[lowerIndex]) * weight);
    }

    private static string ResolveCountryFlag(string countryName)
        => CountryFlagByCountryName.TryGetValue(countryName, out var flag) ? flag : "🏁";

    private static string ResolveTeamColor(string constructorId)
        => TeamColorByConstructorId.TryGetValue(constructorId, out var color) ? color : "FFFFFF";

    private sealed record RaceContext(
        IReadOnlyList<Domain.Entities.Lap> Laps,
        ILookup<string, Domain.Entities.Lap> LapsByDriverId,
        IReadOnlyList<Domain.Entities.RaceResult> Results,
        IReadOnlyDictionary<string, Domain.Entities.RaceResult> ResultsByDriverId,
        ILookup<string, Domain.Entities.PitStop> PitStopsByDriverId);

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
