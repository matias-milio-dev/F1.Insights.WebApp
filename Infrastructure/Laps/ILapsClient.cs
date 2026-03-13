using F1.Insights.App.Domain.Entities;

namespace F1.Insights.App.Infrastructure.Laps;

/// <summary>
/// Retrieves lap data from the Ergast laps endpoint.
/// </summary>
public interface ILapsClient
{
    /// <summary>
    /// Retrieves laps for a specific race round.
    /// </summary>
    Task<IReadOnlyList<Lap>> GetByYearAndRoundAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default);
}