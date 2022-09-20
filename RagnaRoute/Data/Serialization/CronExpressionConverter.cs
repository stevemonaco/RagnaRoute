using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cronos;

namespace RagnaRoute.Data;
public class CronExpressionConverter : JsonConverter<CronExpression>
{
    public override CronExpression? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var contents = reader.GetString();

        if (contents is null)
            return null;

        try
        {
            var expression = CronExpression.Parse(contents, CronFormat.IncludeSeconds);
            return expression;
        }
        catch (CronFormatException ex)
        {
            throw new JsonException($"Could not parse Cron expression: '{contents}'", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, CronExpression value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
