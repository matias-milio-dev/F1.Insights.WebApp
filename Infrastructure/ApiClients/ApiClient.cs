using System.Net.Http.Json;

namespace F1.Insights.App.Infrastructure.ApiClients;

public class ApiClient(IHttpClientFactory httpClientFactory) : IApiClient
{
    private const string OpenF1ClientName = "OpenF1";
    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);

        var client = httpClientFactory.CreateClient(OpenF1ClientName);
        return await client.GetFromJsonAsync<T>(endpoint.TrimStart('/'), cancellationToken);
    }
}

