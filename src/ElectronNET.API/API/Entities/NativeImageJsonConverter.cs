using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

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
            var dict = serializer.Deserialize<Dictionary<float, string>>(reader);
            var newDictionary = new Dictionary<float, Image>();
            foreach (var item in dict)
            {
                var bytes = Convert.FromBase64String(item.Value);
                newDictionary.Add(item.Key, Image.FromStream(new MemoryStream(bytes)));
            }
            return new NativeImage(newDictionary);
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(NativeImage);
    }
}
