using ElectronNET.API.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectronNET.API.Serialization
{
    internal static class ElectronJson
    {
        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
    }

    // Use source generation where feasible for hot paths
    [JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(TrayClickEventArgs))]
    [JsonSerializable(typeof(Rectangle))]
    [JsonSerializable(typeof(Display))]
    [JsonSerializable(typeof(UpdateInfo))]
    [JsonSerializable(typeof(ProgressInfo))]
    [JsonSerializable(typeof(UpdateCheckResult))]
    [JsonSerializable(typeof(SemVer))]
    internal partial class ElectronJsonContext : JsonSerializerContext
    {
    }
}

