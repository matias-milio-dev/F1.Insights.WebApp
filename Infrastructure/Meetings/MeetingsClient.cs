using F1.Insights.App.Domain.Entities;
using F1.Insights.App.Infrastructure.ApiClients;

namespace F1.Insights.App.Infrastructure.Meetings;

public sealed class MeetingsClient(IApiClient apiClient) : IMeetingsClient
{
    public async Task<IReadOnlyList<Meeting>> GetByYearAndCountryAsync(
        int year,
        string countryName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(countryName);

        var endpoint = $"meetings?year={year}&country_name={Uri.EscapeDataString(countryName)}";

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