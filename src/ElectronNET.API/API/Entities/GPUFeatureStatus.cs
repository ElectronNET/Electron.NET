
using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class GPUFeatureStatus
    {
        /// <summary>
        /// Canvas.
        /// </summary>
        [JsonPropertyName("2d_canvas")]
        public string Canvas { get; set; }

        /// <summary>
        /// Flash.
        /// </summary>
        [JsonPropertyName("flash_3d")]
        public string Flash3D { get; set; }

        /// <summary>
        /// Flash Stage3D.
        /// </summary>
        [JsonPropertyName("flash_stage3d")]
        public string FlashStage3D { get; set; }

        /// <summary>
        /// Flash Stage3D Baseline profile.
        /// </summary>
        [JsonPropertyName("flash_stage3d_baseline")]
        public string FlashStage3dBaseline { get; set; }

        /// <summary>
        /// Compositing.
        /// </summary>
        [JsonPropertyName("gpu_compositing")]
        public string GpuCompositing { get; set; }

        /// <summary>
        /// Multiple Raster Threads.
        /// </summary>
        [JsonPropertyName("multiple_raster_threads")]
        public string MultipleRasterThreads { get; set; }

        /// <summary>
        /// Native GpuMemoryBuffers.
        /// </summary>
        [JsonPropertyName("native_gpu_memory_buffers")]
        public string NativeGpuMemoryBuffers { get; set; }

        /// <summary>
        /// Rasterization.
        /// </summary>
        public string Rasterization { get; set; }

        /// <summary>
        /// Video Decode.
        /// </summary>
        [JsonPropertyName("video_decode")]
        public string VideoDecode { get; set; }

        /// <summary>
        /// Video Encode.
        /// </summary>
        [JsonPropertyName("video_encode")]
        public string VideoEncode { get; set; }

        /// <summary>
        /// VPx Video Decode.
        /// </summary>
        [JsonPropertyName("vpx_decode")]
        public string VpxDecode { get; set; }

        /// <summary>
        /// WebGL.
        /// </summary>
        public string Webgl { get; set; }

        /// <summary>
        /// WebGL2.
        /// </summary>
        public string Webgl2 { get; set; }
    }
}