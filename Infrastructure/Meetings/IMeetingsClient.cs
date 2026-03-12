using F1.Insights.App.Domain.Entities;

namespace F1.Insights.App.Infrastructure.Meetings;

public interface IMeetingsClient
{
    Task<IReadOnlyList<Meeting>> GetByYearAsync(
        int year,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Meeting>> GetByYearAndCountryAsync(
        int year,
        string countryName,
        CancellationToken cancellationToken = default);
}