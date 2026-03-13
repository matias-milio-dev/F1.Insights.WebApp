using F1.Insights.App.Domain.Entities;

namespace F1.Insights.App.Infrastructure.Results;

/// <summary>
/// Retrieves race result data from the Ergast results endpoint.
/// </summary>
public interface IResultsClient
{
    /// <summary>
    /// Retrieves race results for a specific year and round.
    /// </summary>
    Task<IReadOnlyList<RaceResult>> GetByYearAndRoundAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default);
}
