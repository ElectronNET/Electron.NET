using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SocketIOClient.JsonSerializer
{
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        public JsonSerializeResult Serialize(object[] data)
        {
            var converter = new ByteArrayConverter();
            var options = GetOptions();
            options.Converters.Add(converter);
            string json = System.Text.Json.JsonSerializer.Serialize(data, options);
            return new JsonSerializeResult
            {
                Json = json,
                Bytes = converter.Bytes
            };
        }

        public T Deserialize<T>(string json)
        {
            var options = GetOptions();
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
        }

        public T Deserialize<T>(string json, IList<byte[]> bytes)
        {
            var options = GetOptions();
            var converter = new ByteArrayConverter();
            options.Converters.Add(converter);
            converter.Bytes.AddRange(bytes);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
        }

        private JsonSerializerOptions GetOptions()
        {
            JsonSerializerOptions options = null;
            if (OptionsProvider != null)
            {
                options = OptionsProvider();
            }
            if (options == null)
            {
                options = new JsonSerializerOptions();
            }
            return options;
        }

        public Func<JsonSerializerOptions> OptionsProvider { get; set; }
    }
}
