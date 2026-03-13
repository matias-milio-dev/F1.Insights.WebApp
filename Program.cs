
using F1.Insights.App;
using F1.Insights.App.Infrastructure.ApiClients;
using F1.Insights.App.Infrastructure.Laps;
using F1.Insights.App.Infrastructure.Meetings;
using F1.Insights.App.Infrastructure.Pit;
using F1.Insights.App.Infrastructure.Results;
using F1.Insights.App.Infrastructure.Sessions;
using F1.Insights.App.Features.GrandPrixSelection;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var ergastBaseUrl = builder.Configuration["ApiClients:Ergast:BaseUrl"]
    ?? throw new InvalidOperationException("Missing configuration: ApiClients:Ergast:BaseUrl");

builder.Services.AddHttpClient("Ergast", client =>
{
    client.BaseAddress = new Uri(ergastBaseUrl);
});

builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<ISessionsClient, SessionsClient>();
builder.Services.AddScoped<IMeetingsClient, MeetingsClient>();
builder.Services.AddScoped<ILapsClient, LapsClient>();
builder.Services.AddScoped<IPitStopClient, PitStopClient>();
builder.Services.AddScoped<IResultsClient, ResultsClient>();
builder.Services.AddScoped<IGrandPrixSelectionService, GrandPrixSelectionService>();

await builder.Build().RunAsync();
