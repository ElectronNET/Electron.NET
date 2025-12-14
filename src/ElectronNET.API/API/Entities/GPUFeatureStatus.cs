using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Graphics Feature Status from chrome://gpu/ as returned by app.getGPUFeatureStatus().
    /// Each field reflects the status of a GPU capability reported by Chromium.
    /// 
    /// Possible values for all fields:
    /// - disabled_software: Software only. Hardware acceleration disabled (yellow)
    /// - disabled_off: Disabled (red)
    /// - disabled_off_ok: Disabled (yellow)
    /// - unavailable_software: Software only, hardware acceleration unavailable (yellow)
    /// - unavailable_off: Unavailable (red)
    /// - unavailable_off_ok: Unavailable (yellow)
    /// - enabled_readback: Hardware accelerated but at reduced performance (yellow)
    /// - enabled_force: Hardware accelerated on all pages (green)
    /// - enabled: Hardware accelerated (green)
    /// - enabled_on: Enabled (green)
    /// - enabled_force_on: Force enabled (green)
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class GPUFeatureStatus
    {
        /// <summary>
        /// Gets or sets the status for Canvas.
        /// </summary>
        [JsonPropertyName("2d_canvas")]
        public string Canvas { get; set; }

        /// <summary>
        /// Gets or sets the status for Flash.
        /// </summary>
        [JsonPropertyName("flash_3d")]
        public string Flash3D { get; set; }

        /// <summary>
        /// Gets or sets the status for Flash Stage3D.
        /// </summary>
        [JsonPropertyName("flash_stage3d")]
        public string FlashStage3D { get; set; }

        /// <summary>
        /// Gets or sets the status for Flash Stage3D Baseline profile.
        /// </summary>
        [JsonPropertyName("flash_stage3d_baseline")]
        public string FlashStage3dBaseline { get; set; }

        /// <summary>
        /// Gets or sets the status for Compositing.
        /// </summary>
        [JsonPropertyName("gpu_compositing")]
        public string GpuCompositing { get; set; }

        /// <summary>
        /// Gets or sets the status for Multiple Raster Threads.
        /// </summary>
        [JsonPropertyName("multiple_raster_threads")]
        public string MultipleRasterThreads { get; set; }

        /// <summary>
        /// Gets or sets the status for Native GpuMemoryBuffers.
        /// </summary>
        [JsonPropertyName("native_gpu_memory_buffers")]
        public string NativeGpuMemoryBuffers { get; set; }

        /// <summary>
        /// Gets or sets the status for Rasterization.
        /// </summary>
        public string Rasterization { get; set; }

        /// <summary>
        /// Gets or sets the status for Video Decode.
        /// </summary>
        [JsonPropertyName("video_decode")]
        public string VideoDecode { get; set; }

        /// <summary>
        /// Gets or sets the status for Video Encode.
        /// </summary>
        [JsonPropertyName("video_encode")]
        public string VideoEncode { get; set; }

        /// <summary>
        /// Gets or sets the status for VPx Video Decode.
        /// </summary>
        [JsonPropertyName("vpx_decode")]
        public string VpxDecode { get; set; }

        /// <summary>
        /// Gets or sets the status for WebGL.
        /// </summary>
        public string Webgl { get; set; }

        /// <summary>
        /// Gets or sets the status for WebGL2.
        /// </summary>
        public string Webgl2 { get; set; }
    }
}