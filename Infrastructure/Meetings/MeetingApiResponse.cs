using System.Text.Json.Serialization;

namespace F1.Insights.App.Infrastructure.Meetings;

internal sealed record MeetingApiResponse(
    [property: JsonPropertyName("circuit_key")] int CircuitKey,
    [property: JsonPropertyName("circuit_info_url")] string CircuitInfoUrl,
    [property: JsonPropertyName("circuit_image")] string CircuitImage,
    [property: JsonPropertyName("circuit_short_name")] string CircuitShortName,
    [property: JsonPropertyName("circuit_type")] string CircuitType,
    [property: JsonPropertyName("country_code")] string CountryCode,
    [property: JsonPropertyName("country_flag")] string CountryFlag,
    [property: JsonPropertyName("country_key")] int CountryKey,
    [property: JsonPropertyName("country_name")] string CountryName,
    [property: JsonPropertyName("date_end")] DateTimeOffset DateEnd,
    [property: JsonPropertyName("date_start")] DateTimeOffset DateStart,
    [property: JsonPropertyName("gmt_offset")] string GmtOffset,
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("meeting_key")] int MeetingKey,
    [property: JsonPropertyName("meeting_name")] string MeetingName,
    [property: JsonPropertyName("meeting_official_name")] string MeetingOfficialName,
    [property: JsonPropertyName("year")] int Year);