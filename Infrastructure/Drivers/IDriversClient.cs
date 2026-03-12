using F1.Insights.App.Domain.Entities;

namespace F1.Insights.App.Infrastructure.Drivers;

public interface IDriversClient
{
    Task<IReadOnlyList<Driver>> GetByDriverNumberAndSessionKeyAsync(
        int driverNumber,
        int sessionKey,
        CancellationToken cancellationToken = default);
}