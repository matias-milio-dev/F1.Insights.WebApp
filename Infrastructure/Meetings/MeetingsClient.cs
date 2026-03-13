using F1.Insights.App.Domain.Entities;
using F1.Insights.App.Infrastructure.ApiClients;
using System.Text.Json;

namespace F1.Insights.App.Infrastructure.Meetings;

/// <summary>
/// Retrieves race data from the Ergast races endpoint.
/// </summary>
public sealed class MeetingsClient(IApiClient apiClient) : IMeetingsClient
{
    public async Task<IReadOnlyList<Meeting>> GetByYearAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"{year}/races/";
        using var response = await apiClient.GetAsync<JsonDocument>(endpoint, cancellationToken);

        if (response is null)
        {
            return [];
        }

        if (!response.RootElement.TryGetProperty("MRData", out var mrData) ||
            !mrData.TryGetProperty("RaceTable", out var raceTable) ||
            !raceTable.TryGetProperty("Races", out var races) ||
            races.ValueKind is not JsonValueKind.Array)
        {
            return [];
        }

        return [.. races
            .EnumerateArray()
            .Select(race =>
            {
                var raceName = race.GetProperty("raceName").GetString() ?? "Unknown Race";
                var round = ParseInt(race.GetProperty("round").GetString());
                var date = race.GetProperty("date").GetString();
                var time = race.TryGetProperty("time", out var timeEl) ? timeEl.GetString() : null;

                var dateStart = ParseDateTimeOffset(date, time);

                var circuit = race.GetProperty("Circuit");
                var location = circuit.GetProperty("Location");

                var countryName = location.GetProperty("country").GetString() ?? "Unknown";
                var circuitName = circuit.GetProperty("circuitName").GetString() ?? "Unknown Circuit";
                var locality = location.GetProperty("locality").GetString() ?? "Unknown";

                return new Meeting(
                    raceName,
                    round,
                    countryName,
                    dateStart,
                    circuitName,
                    locality,
                    year);
            })
            .OrderBy(meeting => meeting.Round)];
    }

    private static int ParseInt(string? value)
        => int.TryParse(value, out var parsed) ? parsed : 0;

    private static DateTimeOffset ParseDateTimeOffset(string? date, string? time)
    {
        var combined = string.IsNullOrWhiteSpace(time) ? date : $"{date}T{time}";

        return DateTimeOffset.TryParse(combined, out var parsed)
            ? parsed
            : DateTimeOffset.MinValue;
    }
}