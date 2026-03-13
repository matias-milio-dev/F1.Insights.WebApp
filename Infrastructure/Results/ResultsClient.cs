using System.Globalization;
using System.Text.Json;
using F1.Insights.App.Domain.Entities;
using F1.Insights.App.Infrastructure.ApiClients;

namespace F1.Insights.App.Infrastructure.Results;

/// <summary>
/// Retrieves race result data from the Ergast results endpoint.
/// </summary>
public sealed class ResultsClient(IApiClient apiClient) : IResultsClient
{
    public async Task<IReadOnlyList<RaceResult>> GetByYearAndRoundAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"{year}/{round}/results/";
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

        if (!race.TryGetProperty("Results", out var results) ||
            results.ValueKind is not JsonValueKind.Array)
        {
            return [];
        }

        return [.. results
            .EnumerateArray()
            .Select(result =>
            {
                var position = ParseInt(result.GetProperty("position").GetString());

                var driver = result.GetProperty("Driver");
                var constructor = result.GetProperty("Constructor");

                var driverId = driver.GetProperty("driverId").GetString() ?? string.Empty;
                var code = driver.TryGetProperty("code", out var codeEl) ? codeEl.GetString() ?? string.Empty : string.Empty;
                var givenName = driver.GetProperty("givenName").GetString() ?? string.Empty;
                var familyName = driver.GetProperty("familyName").GetString() ?? string.Empty;
                var driverNumber = ParseNullableInt(driver.TryGetProperty("permanentNumber", out var numberEl) ? numberEl.GetString() : null);

                var constructorId = constructor.GetProperty("constructorId").GetString() ?? string.Empty;
                var teamName = constructor.GetProperty("name").GetString() ?? string.Empty;

                int? fastestLapNumber = null;
                double? fastestLapTimeSeconds = null;

                if (result.TryGetProperty("FastestLap", out var fastestLap))
                {
                    fastestLapNumber = ParseNullableInt(fastestLap.TryGetProperty("lap", out var lapEl) ? lapEl.GetString() : null);

                    if (fastestLap.TryGetProperty("Time", out var fastestTime) &&
                        fastestTime.TryGetProperty("time", out var fastestTimeValue))
                    {
                        fastestLapTimeSeconds = ParseLapTimeToSeconds(fastestTimeValue.GetString());
                    }
                }

                return new RaceResult(
                    driverId,
                    code,
                    $"{givenName} {familyName}".Trim(),
                    driverNumber,
                    constructorId,
                    teamName,
                    position,
                    fastestLapNumber,
                    fastestLapTimeSeconds,
                    round,
                    year);
            })];
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
