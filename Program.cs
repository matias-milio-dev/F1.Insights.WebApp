
using F1.Insights.App;
using F1.Insights.App.Infrastructure.ApiClients;
using F1.Insights.App.Infrastructure.Drivers;
using F1.Insights.App.Infrastructure.Meetings;
using F1.Insights.App.Infrastructure.Sessions;
using F1.Insights.App.Features.GrandPrixSelection;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

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
builder.Services.AddScoped<IGrandPrixSelectionService, GrandPrixSelectionService>();

await builder.Build().RunAsync();
