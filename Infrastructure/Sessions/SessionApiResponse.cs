using System.Text.Json.Serialization;

namespace F1.Insights.App.Infrastructure.Sessions;

internal sealed record SessionApiResponse(
    [property: JsonPropertyName("circuit_key")] int CircuitKey,
    [property: JsonPropertyName("circuit_short_name")] string CircuitShortName,
    [property: JsonPropertyName("country_code")] string CountryCode,
    [property: JsonPropertyName("country_key")] int CountryKey,
    [property: JsonPropertyName("country_name")] string CountryName,
    [property: JsonPropertyName("date_end")] DateTimeOffset DateEnd,
    [property: JsonPropertyName("date_start")] DateTimeOffset DateStart,
    [property: JsonPropertyName("gmt_offset")] string GmtOffset,
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("meeting_key")] int MeetingKey,
    [property: JsonPropertyName("session_key")] int SessionKey,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("session_type")] string SessionType,
    [property: JsonPropertyName("year")] int Year);