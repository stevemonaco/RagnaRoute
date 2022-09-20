using NodaTime;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RagnaRoute.Data;

public class DateTimeZoneConverter : JsonConverter<DateTimeZone>
{
    public override DateTimeZone? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var ianaZoneString = reader.GetString();

        if (ianaZoneString is null)
            return null;

        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ianaZoneString);
        
        if (zone is null)
            throw new JsonException($"Unrecognized IANA time zone: '{ianaZoneString}'");

        return zone;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeZone value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Id);
    }
}
