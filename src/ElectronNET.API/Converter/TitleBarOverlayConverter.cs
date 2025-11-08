using ElectronNET.API.Entities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectronNET.Converter;

public class TitleBarOverlayConverter : JsonConverter<TitleBarOverlay>
{
    public override TitleBarOverlay Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.StartObject => JsonSerializer.Deserialize<TitleBarOverlay>(ref reader, options),
            _ => throw new JsonException("Invalid value for TitleBarOverlay. Expected true or false."),
        };
    }

    public override void Write(Utf8JsonWriter writer, TitleBarOverlay value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var @bool = (bool?)value;
        if (@bool.HasValue)
        {
            writer.WriteBooleanValue(@bool.Value);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}