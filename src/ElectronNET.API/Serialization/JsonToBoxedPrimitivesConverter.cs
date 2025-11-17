namespace ElectronNET.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public sealed class JsonToBoxedPrimitivesConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadValue(ref reader);
        }

        private static object ReadValue(ref Utf8JsonReader r)
        {
            switch (r.TokenType)
            {
                case JsonTokenType.StartObject:

                    var obj = new Dictionary<string, object>();
                    while (r.Read())
                    {
                        if (r.TokenType == JsonTokenType.EndObject)
                        {
                            return obj;
                        }

                        if (r.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonException("Expected property name.");
                        }

                        string name = r.GetString()!;
                        if (!r.Read())
                        {
                            throw new JsonException("Unexpected end while reading property value.");
                        }

                        obj[name] = ReadValue(ref r);
                    }

                    throw new JsonException("Unexpected end while reading object.");

                case JsonTokenType.StartArray:

                    var list = new List<object>();
                    while (r.Read())
                    {
                        if (r.TokenType == JsonTokenType.EndArray)
                        {
                            return list;
                        }

                        list.Add(ReadValue(ref r));
                    }

                    throw new JsonException("Unexpected end while reading array.");

                case JsonTokenType.True: return true;
                case JsonTokenType.False: return false;
                case JsonTokenType.Null: return null;

                case JsonTokenType.Number:

                    if (r.TryGetInt32(out int i))
                    {
                        return i;
                    }

                    if (r.TryGetInt64(out long l))
                    {
                        return l;
                    }

                    if (r.TryGetDouble(out double d))
                    {
                        return d;
                    }

                    return r.GetDecimal();

                case JsonTokenType.String:

                    string s = r.GetString()!;

                    if (DateTimeOffset.TryParse(s, out var dto))
                    {
                        return dto;
                    }

                    if (DateTime.TryParse(s, out var dt))
                    {
                        return dt;
                    }

                    if (TimeSpan.TryParse(s, out var ts))
                    {
                        return ts;
                    }

                    if (Guid.TryParse(s, out var g))
                    {
                        return g;
                    }

                    return s;

                default:
                    throw new JsonException($"Unsupported token {r.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();
            writer.WriteEndObject();
        }
    }
}