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
                        if (reader.TryGetInt64(out var number))
                        {
                            return (TEnum)Enum.ToObject(typeof(TEnum), number);
                        }
                        break;
                }

                throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum {typeof(TEnum).Name}.");
            }

            public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            {
                if (valueToName.TryGetValue(value, out var name))
                {
                    writer.WriteStringValue(name);
                    return;
                }

                // Combined flags or undefined values: fall back to the default enum formatting.
                writer.WriteStringValue(fallbackNames.GetOrAdd(value, v => ConvertName(v.ToString())));
            }

            private string ConvertName(string name)
            {
                return namingPolicy?.ConvertName(name) ?? name;
            }
        }
    }
}
