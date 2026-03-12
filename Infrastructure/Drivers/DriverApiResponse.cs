using System.Text.Json.Serialization;

namespace F1.Insights.App.Infrastructure.Drivers;

internal sealed record DriverApiResponse(
    [property: JsonPropertyName("broadcast_name")] string BroadcastName,
    [property: JsonPropertyName("driver_number")] int DriverNumber,
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("headshot_url")] string HeadshotUrl,
    [property: JsonPropertyName("last_name")] string LastName,
    [property: JsonPropertyName("meeting_key")] int MeetingKey,
    [property: JsonPropertyName("name_acronym")] string NameAcronym,
    [property: JsonPropertyName("session_key")] int SessionKey,
    [property: JsonPropertyName("team_colour")] string TeamColour,
    [property: JsonPropertyName("team_name")] string TeamName);