namespace ElectronNET.Converter;

using System;
using System.Collections.Generic;
using System.Linq;
using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 
/// </summary>
public class ModifierTypeListConverter : JsonConverter<List<ModifierType>>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="objectType"></param>
    /// <param name="existingValue"></param>
    /// <param name="hasExistingValue"></param>
    /// <param name="serializer"></param>
    /// <returns></returns>
    public override List<ModifierType> ReadJson(JsonReader reader, Type objectType, List<ModifierType> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.Null)
        {
            return null;
        }


        return token.ToObject<List<string>>().Select(m => (ModifierType)Enum.Parse(typeof(ModifierType), m)).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="serializer"></param>
    public override void WriteJson(JsonWriter writer, List<ModifierType> value, JsonSerializer serializer)
    {
        writer.WriteStartArray();

        foreach (var modifier in value)
        {
            writer.WriteValue(modifier.ToString());
        }

        writer.WriteEndArray();
    }
}