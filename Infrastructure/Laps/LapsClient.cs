using F1.Insights.App.Domain.Entities;
using F1.Insights.App.Infrastructure.ApiClients;
using System.Globalization;
using System.Text.Json;

namespace F1.Insights.App.Infrastructure.Laps;

/// <summary>
/// Retrieves lap data from the Ergast laps endpoint.
/// </summary>
public sealed class LapsClient(IApiClient apiClient) : ILapsClient
{
    public async Task<IReadOnlyList<Lap>> GetByYearAndRoundAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"{year}/{round}/laps/";
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
        if (!race.TryGetProperty("Laps", out var lapsElement) ||
            lapsElement.ValueKind is not JsonValueKind.Array)
        {
            return [];
        }

        var lapList = new List<Lap>();

        foreach (var lapItem in lapsElement.EnumerateArray())
        {
            var lapNumber = ParseInt(lapItem.GetProperty("number").GetString());

            if (!lapItem.TryGetProperty("Timings", out var timings) ||
                timings.ValueKind is not JsonValueKind.Array)
            {
                continue;
            }

            foreach (var timing in timings.EnumerateArray())
            {
                var driverId = timing.GetProperty("driverId").GetString() ?? string.Empty;
                var position = ParseNullableInt(timing.TryGetProperty("position", out var positionEl)
                    ? positionEl.GetString()
                    : null);

                var lapDuration = ParseLapTimeToSeconds(timing.GetProperty("time").GetString());

                if (lapDuration is null)
                {
                    continue;
                }

                lapList.Add(new Lap(
                    driverId,
                    position,
                    lapDuration.Value,
                    lapNumber,
                    round,
                    year));
            }
        }

        return lapList;
    }

    private static int ParseInt(string? value)
        => int.TryParse(value, out var parsed) ? parsed : 0;

    private static int? ParseNullableInt(string? value)
        => int.TryParse(value, out var parsed) ? parsed : null;

    private static double? ParseLapTimeToSeconds(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var parts = value.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return null;
        }

        if (!int.TryParse(parts[0], out var minutes))
        {
            return null;
        }

        if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds))
        {
            return null;
        }

        return (minutes * 60) + seconds;
    }
}