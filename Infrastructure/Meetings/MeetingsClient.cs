using F1.Insights.App.Domain.Entities;
using F1.Insights.App.Infrastructure.ApiClients;

namespace F1.Insights.App.Infrastructure.Meetings;

/// <summary>
/// Retrieves meeting data from the OpenF1 meetings endpoint.
/// </summary>
public sealed class MeetingsClient(IApiClient apiClient) : IMeetingsClient
{
    public async Task<IReadOnlyList<Meeting>> GetByYearAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"meetings?year={year}";

        return await GetMeetingsAsync(endpoint, cancellationToken);
    }

    public async Task<IReadOnlyList<Meeting>> GetByYearAndCountryAsync(
        int year,
        string countryName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(countryName);

        var endpoint = $"meetings?year={year}&country_name={Uri.EscapeDataString(countryName)}";

        return await GetMeetingsAsync(endpoint, cancellationToken);
    }

    private async Task<IReadOnlyList<Meeting>> GetMeetingsAsync(string endpoint, CancellationToken cancellationToken)
    {
        var response = await apiClient.GetAsync<List<MeetingApiResponse>>(endpoint, cancellationToken)
            ?? [];

        return [.. response
            .Select(static meeting => new Meeting(
                meeting.CircuitKey,
                meeting.CircuitInfoUrl,
                meeting.CircuitImage,
                meeting.CircuitShortName,
                meeting.CircuitType,
                meeting.CountryCode,
                meeting.CountryFlag,
                meeting.CountryKey,
                meeting.CountryName,
                meeting.DateEnd,
                meeting.DateStart,
                meeting.GmtOffset,
                meeting.Location,
                meeting.MeetingKey,
                meeting.MeetingName,
                meeting.MeetingOfficialName,
                meeting.Year))];
    }
}