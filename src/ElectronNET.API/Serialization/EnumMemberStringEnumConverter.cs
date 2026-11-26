using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectronNET.API.Serialization
{
    internal sealed class EnumMemberStringEnumConverter : JsonConverterFactory
    {
        private readonly JsonNamingPolicy namingPolicy;

        public EnumMemberStringEnumConverter(JsonNamingPolicy namingPolicy = null)
        {
            this.namingPolicy = namingPolicy;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(EnumConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter) Activator.CreateInstance(converterType, namingPolicy);
        }

        private sealed class EnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
        {
            private readonly Dictionary<TEnum, string> valueToName = new Dictionary<TEnum, string>();
            private readonly Dictionary<string, TEnum> nameToValue = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);
            private readonly ConcurrentDictionary<TEnum, string> fallbackNames = new ConcurrentDictionary<TEnum, string>();
            private readonly JsonNamingPolicy namingPolicy;

            public EnumConverter(JsonNamingPolicy namingPolicy)
            {
                this.namingPolicy = namingPolicy;

                foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var value = (TEnum) field.GetValue(null);
                    var name = field.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? field.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? ConvertName(field.Name);

                    valueToName.TryAdd(value, name);
                    nameToValue.TryAdd(name, value);
                    nameToValue.TryAdd(field.Name, value);
                }
            }

            public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        var text = reader.GetString();
                        if (text != null && nameToValue.TryGetValue(text, out var value))
                        {
                            return value;
                        }
                        if (Enum.TryParse<TEnum>(text, true, out value))
                        {
                            return value;
                        }
                        throw new JsonException($"Unable to convert \"{text}\" to enum {typeof(TEnum).Name}.");
                    case JsonTokenType.Number:
                        var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
                        var numericValue = JsonSerializer.Deserialize(ref reader, underlyingType, options);
                        return (TEnum)Enum.ToObject(typeof(TEnum), numericValue);
                }

                throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum {typeof(TEnum).Name}.");
            }

            public override TEnum ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var text = reader.GetString();
                if (text != null && (nameToValue.TryGetValue(text, out var value) || Enum.TryParse<TEnum>(text, true, out value)))
                {
                    return value;
                }
                throw new JsonException($"Unable to convert \"{text}\" to enum {typeof(TEnum).Name}.");
            }

            public override void WriteAsPropertyName(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            {
                writer.WritePropertyName(valueToName.TryGetValue(value, out var name) ? name : fallbackNames.GetOrAdd(value, v => ConvertName(v.ToString())));
            }

            public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            {
                if (valueToName.TryGetValue(value, out var name))
                {
                    writer.WriteStringValue(name);
                    return;
                }

                // Combined flags or undefined values: fall back to the default enum formatting.
                var formatted = value.ToString();
                if (char.IsDigit(formatted[0]) || formatted[0] == '-')
                {
                    var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
                    JsonSerializer.Serialize(writer, Convert.ChangeType(value, underlyingType), underlyingType, options);
                    return;
                }
                writer.WriteStringValue(fallbackNames.GetOrAdd(value, v => ConvertName(v.ToString())));
            }

            private string ConvertName(string name)
            {
                return namingPolicy?.ConvertName(name) ?? name;
            }
        }
    }
}
