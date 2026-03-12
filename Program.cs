
using F1.Insights.App.Infrastructure.ApiClients;
using F1.Insights.App.Infrastructure.Drivers;
using F1.Insights.App.Infrastructure.Meetings;
using F1.Insights.App.Infrastructure.Sessions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var openF1BaseUrl = builder.Configuration["ApiClients:OpenF1:BaseUrl"]
    ?? throw new InvalidOperationException("Missing configuration: ApiClients:OpenF1:BaseUrl");

builder.Services.AddHttpClient("OpenF1", client =>
{
    client.BaseAddress = new Uri(openF1BaseUrl);
});

builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<IDriversClient, DriversClient>();
builder.Services.AddScoped<ISessionsClient, SessionsClient>();
builder.Services.AddScoped<IMeetingsClient, MeetingsClient>();

await builder.Build().RunAsync();
