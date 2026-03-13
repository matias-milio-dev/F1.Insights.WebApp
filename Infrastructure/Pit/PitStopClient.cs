using F1.Insights.App.Domain.Entities;
using F1.Insights.App.Infrastructure.ApiClients;
using System.Globalization;
using System.Text.Json;

namespace F1.Insights.App.Infrastructure.Pit;

/// <summary>
/// Retrieves pit stop data from the Ergast pitstops endpoint.
/// </summary>
public sealed class PitStopClient(IApiClient apiClient) : IPitStopClient
{
    public async Task<IReadOnlyList<PitStop>> GetByYearAndRoundAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"{year}/{round}/pitstops/";
        using var response = await apiClient.GetAsync<JsonDocument>(endpoint, cancellationToken);

        if (response is null)
        {
            return [];
        }

        if (!response.RootElement.TryGetProperty("MRData", out var mrData) ||
            !mrData.TryGetProperty("RaceTable", out var raceTable) ||
            !raceTable.TryGetProperty("Races", out var races) ||
            races.ValueKind is not JsonValueKind.Array ||
            races.GetArrayLength() == 0)
        {
            return [];
        }

        var race = races[0];

        if (!race.TryGetProperty("PitStops", out var pitStopsElement) ||
            pitStopsElement.ValueKind is not JsonValueKind.Array)
        {
            return [];
        }

        return [.. pitStopsElement
            .EnumerateArray()
            .Select(pit =>
            {
                var driverId = pit.GetProperty("driverId").GetString() ?? string.Empty;
                var stop = ParseInt(pit.GetProperty("stop").GetString());
                var lap = ParseInt(pit.GetProperty("lap").GetString());
                var duration = ParseDuration(pit.GetProperty("duration").GetString());

                return new PitStop(driverId, stop, lap, round, year, duration);
            })];
    }

    private static int ParseInt(string? value)
        => int.TryParse(value, out var parsed) ? parsed : 0;

    private static TimeSpan? ParseDuration(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds))
        {
            return null;
        }

        return TimeSpan.FromSeconds(seconds);
    }
}