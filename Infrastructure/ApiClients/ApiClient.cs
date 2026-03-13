using System.Net.Http.Json;
using System.Text;

namespace F1.Insights.App.Infrastructure.ApiClients;

/// <summary>
/// Provides generic HTTP access to Ergast endpoints.
/// </summary>
public class ApiClient(
    IHttpClientFactory httpClientFactory,
    ILogger<ApiClient> logger) : IApiClient
{
    private const string ErgastClientName = "Ergast";

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
        try
        {
            var client = httpClientFactory.CreateClient(ErgastClientName);
            return await client.GetFromJsonAsync<T>(endpoint.TrimStart('/'), cancellationToken);
        }
        catch (Exception ex)
        {
            await LogExceptionAsync(endpoint, ex, cancellationToken);
            throw;
        }
    }
    private async Task LogExceptionAsync(string endpoint, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Ergast request failed for endpoint {Endpoint}", endpoint);

        try
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (string.IsNullOrWhiteSpace(documentsPath))
            {
                return;
            }

            var logDirectory = Path.Combine(documentsPath, "F1.Insights.App", "logs");
            Directory.CreateDirectory(logDirectory);

            var logFilePath = Path.Combine(logDirectory, "api-client-errors.log");
            var logEntry = new StringBuilder()
                .AppendLine("----------------------------------------")
                .AppendLine($"Timestamp (UTC): {DateTime.UtcNow:O}")
                .AppendLine($"Endpoint: {endpoint}")
                .AppendLine($"Exception: {exception.GetType().FullName}")
                .AppendLine($"Message: {exception.Message}")
                .AppendLine($"StackTrace: {exception.StackTrace}")
                .ToString();

            await File.AppendAllTextAsync(logFilePath, logEntry, cancellationToken);
        }
        catch (Exception logException)
        {
            logger.LogError(logException, "Failed to write API client error log to Documents folder.");
        }
    }
}

