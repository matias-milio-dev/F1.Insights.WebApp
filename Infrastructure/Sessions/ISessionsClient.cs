using F1.Insights.App.Domain.Entities;

namespace F1.Insights.App.Infrastructure.Sessions;

public interface ISessionsClient
{
    Task<IReadOnlyList<Session>> GetByYearAndRoundAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default);
}