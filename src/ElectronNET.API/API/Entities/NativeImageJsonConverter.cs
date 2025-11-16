using ElectronNET.API.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    internal class NativeImageJsonConverter : JsonConverter<NativeImage>
    {
        public override void Write(Utf8JsonWriter writer, NativeImage value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            var scaledImages = value.GetAllScaledImages();
            JsonSerializer.Serialize(writer, scaledImages, ElectronJson.Options);
        }

        public override NativeImage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<float, string>>(ref reader, ElectronJson.Options);
            var newDictionary = new Dictionary<float, Image>();
            foreach (var item in dict)
            {
                var bytes = Convert.FromBase64String(item.Value);
                newDictionary.Add(item.Key, Image.FromStream(new MemoryStream(bytes)));
            }

            return new NativeImage(newDictionary);
        }
    }
}