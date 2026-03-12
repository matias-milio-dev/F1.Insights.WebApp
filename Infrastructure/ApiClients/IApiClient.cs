namespace F1.Insights.App.Infrastructure.ApiClients
{
    public interface IApiClient
    {
        Task<T?>GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    }
}
