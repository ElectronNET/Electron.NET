using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketIOClient.Newtonsoft.Json
{
    class ByteArrayConverter : JsonConverter
    {
        public ByteArrayConverter()
        {
            Bytes = new List<byte[]>();
        }

        internal List<byte[]> Bytes { get; }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
        {
            byte[] bytes = null;
            if (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.PropertyName && reader.Value?.ToString() == "_placeholder")
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.Boolean && (bool)reader.Value)
                    {
                        reader.Read();
                        if (reader.TokenType == JsonToken.PropertyName && reader.Value?.ToString() == "num")
                        {
                            reader.Read();
                            if (reader.Value != null)
                            {
                                if (int.TryParse(reader.Value.ToString(), out int num))
                                {
                                    bytes = Bytes[num];
                                    reader.Read();
                                }
                            }
                        }
                    }
                }
            }
            return bytes;
        }

        public override void WriteJson(JsonWriter writer, object value, global::Newtonsoft.Json.JsonSerializer serializer)
        {
            var source = value as byte[];
            Bytes.Add(source.ToArray());
            writer.WriteStartObject();
            writer.WritePropertyName("_placeholder");
            writer.WriteValue(true);
            writer.WritePropertyName("num");
            writer.WriteValue(Bytes.Count - 1);
            writer.WriteEndObject();
        }
    }
}
