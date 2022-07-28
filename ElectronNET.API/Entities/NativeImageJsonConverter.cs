using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.IO;
using Newtonsoft.Json;
using SixLabors.ImageSharp;

namespace ElectronNET.API.Entities
{
    internal class NativeImageJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is NativeImage nativeImage)
            {
                var scaledImages = nativeImage.GetAllScaledImages();
                serializer.Serialize(writer, scaledImages);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dict = serializer.Deserialize<Dictionary<string, string>>(reader);
            var newDictionary = new Dictionary<float, Image>();
            foreach (var item in dict)
            {
                if (float.TryParse(item.Key, out var size))
                {
                    var bytes = Convert.FromBase64String(item.Value);
                    newDictionary.Add(size, Image.Load(new MemoryStream(bytes)));
                }
            }
            return new NativeImage(newDictionary);
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(NativeImage);
    }
}
