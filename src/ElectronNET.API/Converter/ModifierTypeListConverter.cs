namespace ElectronNET.Converter;

using ElectronNET.API.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 
/// </summary>
public class ModifierTypeListConverter : JsonConverter<List<ModifierType>>
{
    public override List<ModifierType> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var list = new List<ModifierType>();
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected array for ModifierType list");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;
            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string enum value");
            var s = reader.GetString();
            list.Add((ModifierType)Enum.Parse(typeof(ModifierType), s, ignoreCase: true));
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, List<ModifierType> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var modifier in value)
        {
            writer.WriteStringValue(modifier.ToString());
        }

        writer.WriteEndArray();
    }
}