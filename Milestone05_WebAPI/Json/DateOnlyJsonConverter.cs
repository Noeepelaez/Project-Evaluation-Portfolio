using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milestone05_WebAPI.Json;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public const string Format = "dd-MM-yyyy";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String
            && DateOnly.TryParseExact(reader.GetString(), Format, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateOnly date))
        {
            return date;
        }

        throw new JsonException($"Invalid date. Use the format {Format}, e.g. 19-11-2026.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
