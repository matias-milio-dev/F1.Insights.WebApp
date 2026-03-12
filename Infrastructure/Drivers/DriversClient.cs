using F1.Insights.App.Infrastructure.ApiClients;
using F1.Insights.App.Domain.Entities;

namespace F1.Insights.App.Infrastructure.Drivers;

public sealed class DriversClient(IApiClient apiClient) : IDriversClient
{
    public async Task<IReadOnlyList<Driver>> GetByDriverNumberAndSessionKeyAsync(
        int driverNumber,
        int sessionKey,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"drivers?driver_number={driverNumber}&session_key={sessionKey}";
        var response = await apiClient.GetAsync<List<DriverApiResponse>>(endpoint, cancellationToken)
            ?? [];

        return [.. response
            .Select(static driver => new Driver(
                driver.BroadcastName,
                driver.DriverNumber,
                driver.FirstName,
                driver.FullName,
                driver.HeadshotUrl,
                driver.LastName,
                driver.MeetingKey,
                driver.NameAcronym,
                driver.SessionKey,
                driver.TeamColour,
                driver.TeamName))];
    }
}