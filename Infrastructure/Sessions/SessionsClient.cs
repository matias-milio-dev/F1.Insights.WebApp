using F1.Insights.App.Domain.Entities;
using F1.Insights.App.Infrastructure.ApiClients;
using System.Text.Json;

namespace F1.Insights.App.Infrastructure.Sessions;

/// <summary>
/// Retrieves race weekend session schedule data from the Ergast races endpoint.
/// </summary>
public sealed class SessionsClient(IApiClient apiClient) : ISessionsClient
{
    public async Task<IReadOnlyList<Session>> GetByYearAndRoundAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"{year}/{round}/races/";
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
        var raceDate = race.GetProperty("date").GetString();
        var raceTime = race.TryGetProperty("time", out var raceTimeEl) ? raceTimeEl.GetString() : null;

        var sessions = new Session?[]
        {
            CreateSession(race, "FirstPractice", "FP1", "Practice 1", year, round),
            CreateSession(race, "SecondPractice", "FP2", "Practice 2", year, round),
            CreateSession(race, "ThirdPractice", "FP3", "Practice 3", year, round),
            CreateSession(race, "Qualifying", "Qualifying", "Qualifying", year, round),
            new Session(
                round,
                BuildSessionKey(round, "Race"),
                "Race",
                "Race",
                ParseDateTimeOffset(raceDate, raceTime),
                year)
        };

        return sessions
            .Where(static session => session is not null)
            .Select(static session => session!)
            .OrderBy(session => session.DateStart)
            .ToArray();
    }

    private static Session? CreateSession(
        JsonElement race,
        string propertyName,
        string label,
        string sessionType,
        int year,
        int round)
    {
        if (!race.TryGetProperty(propertyName, out var sessionElement))
        {
            return null;
        }

        var date = sessionElement.GetProperty("date").GetString();
        var time = sessionElement.TryGetProperty("time", out var timeElement)
            ? timeElement.GetString()
            : null;

        return new Session(
            round,
            BuildSessionKey(round, label),
            label,
            sessionType,
            ParseDateTimeOffset(date, time),
            year);
    }

    private static int BuildSessionKey(int round, string label)
        => HashCode.Combine(round, label);

    private static DateTimeOffset ParseDateTimeOffset(string? date, string? time)
    {
        var combined = string.IsNullOrWhiteSpace(time) ? date : $"{date}T{time}";

        return DateTimeOffset.TryParse(combined, out var parsed)
            ? parsed
            : DateTimeOffset.MinValue;
    }
}