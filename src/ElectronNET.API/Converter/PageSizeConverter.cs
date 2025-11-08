using ElectronNET.API.Entities;
using Newtonsoft.Json;
using System;

namespace ElectronNET.Converter;

public class PageSizeConverter : JsonConverter<PageSize>
{
    public override PageSize ReadJson(JsonReader reader, Type objectType, PageSize existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            return (string)reader.Value;
        }
        else if (reader.TokenType == JsonToken.StartObject)
        {
            return serializer.Deserialize<PageSize>(reader);
        }
        else
        {
            throw new JsonSerializationException("Invalid value for PageSize. Expected true, false, or an object.");
        }
    }

    public override void WriteJson(JsonWriter writer, PageSize value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteUndefined();
        }

        var str = (string)value;

        if (str is not null)
        {
            writer.WriteValue(str);
        }
        else
        {
            serializer.Serialize(writer, value);
        }
    }
}
