using F1.Insights.App.Domain.Entities;
using F1.Insights.App.Infrastructure.ApiClients;

namespace F1.Insights.App.Infrastructure.Sessions;

public sealed class SessionsClient(IApiClient apiClient) : ISessionsClient
{
    public async Task<IReadOnlyList<Session>> GetByCountrySessionNameAndYearAsync(
        string countryName,
        string sessionName,
        int year,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(countryName);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionName);

        var endpoint =
            $"sessions?country_name={Uri.EscapeDataString(countryName)}&session_name={Uri.EscapeDataString(sessionName)}&year={year}";

        var response = await apiClient.GetAsync<List<SessionApiResponse>>(endpoint, cancellationToken)
            ?? [];

        return [.. response
            .Select(static session => new Session(
                session.CircuitKey,
                session.CircuitShortName,
                session.CountryCode,
                session.CountryKey,
                session.CountryName,
                session.DateEnd,
                session.DateStart,
                session.GmtOffset,
                session.Location,
                session.MeetingKey,
                session.SessionKey,
                session.SessionName,
                session.SessionType,
                session.Year))];
    }
}