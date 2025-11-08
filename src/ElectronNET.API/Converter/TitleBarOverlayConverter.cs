using ElectronNET.API.Entities;
using Newtonsoft.Json;
using System;

namespace ElectronNET.Converter;

public class TitleBarOverlayConverter : JsonConverter<TitleBarOverlay>
{
    public override TitleBarOverlay ReadJson(JsonReader reader, Type objectType, TitleBarOverlay existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Boolean)
        {
            return (bool)reader.Value;
        }
        else if (reader.TokenType == JsonToken.StartObject)
        {
            return serializer.Deserialize<TitleBarOverlay>(reader);
        }
        else
        {
            throw new JsonSerializationException("Invalid value for TitleBarOverlay. Expected true, false, or an object.");
        }
    }

    public override void WriteJson(JsonWriter writer, TitleBarOverlay value, JsonSerializer serializer)
    {
        if (value is null)
        {
            return;
        }

        var @bool = (bool?)value;
        if (@bool.HasValue)
        {
            writer.WriteValue(@bool.Value);
        }
        else
        {
            serializer.Serialize(writer, value);
        }
    }
}