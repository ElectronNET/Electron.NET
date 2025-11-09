using ElectronNET.API.Entities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectronNET.Converter;

public class PageSizeConverter : JsonConverter<PageSize>
{
    public override PageSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return (string)reader.GetString();
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return doc.RootElement.Deserialize<PageSize>(API.Serialization.ElectronJson.Options);
        }
        else
        {
            throw new JsonException("Invalid value for PageSize. Expected string or an object.");
        }
    }

    public override void Write(Utf8JsonWriter writer, PageSize value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var str = (string)value;

        if (str is not null)
        {
            writer.WriteStringValue(str);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, API.Serialization.ElectronJson.Options);
        }
    }
}

