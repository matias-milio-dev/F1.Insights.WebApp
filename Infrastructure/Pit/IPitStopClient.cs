using F1.Insights.App.Domain.Entities;

namespace F1.Insights.App.Infrastructure.Pit;

/// <summary>
/// Retrieves pit stop data from the Ergast pitstops endpoint.
/// </summary>
public interface IPitStopClient
{
    /// <summary>
    /// Retrieves pit stops for a specific race round.
    /// </summary>
    Task<IReadOnlyList<PitStop>> GetByYearAndRoundAsync(
        int year,
        int round,
        CancellationToken cancellationToken = default);
}