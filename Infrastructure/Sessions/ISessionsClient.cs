using F1.Insights.App.Domain.Entities;

namespace F1.Insights.App.Infrastructure.Sessions;

public interface ISessionsClient
{
    Task<IReadOnlyList<Session>> GetByCountrySessionNameAndYearAsync(
        string countryName,
        string sessionName,
        int year,
        CancellationToken cancellationToken = default);
}