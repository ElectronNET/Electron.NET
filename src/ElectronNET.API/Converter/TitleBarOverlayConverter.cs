using ElectronNET.API.Entities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectronNET.Converter;

public class TitleBarOverlayConverter : JsonConverter<TitleBarOverlay>
{
    public override TitleBarOverlay Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
        {
            return (bool)reader.GetBoolean();
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return doc.RootElement.Deserialize<TitleBarOverlay>(API.Serialization.ElectronJson.Options);
        }
        else
        {
            throw new JsonException("Invalid value for TitleBarOverlay. Expected boolean or an object.");
        }
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
            JsonSerializer.Serialize(writer, value, API.Serialization.ElectronJson.Options);
        }
    }
}

