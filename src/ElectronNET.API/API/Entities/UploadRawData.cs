namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class UploadRawData : IPostData
    {
        /// <summary>
        /// Gets the type discriminator; constant 'rawData'.
        /// </summary>
        public string Type { get; } = "rawData";

        /// <summary>
        /// Gets or sets the data to be uploaded as raw bytes (Electron Buffer).
        /// </summary>
        public byte[] Bytes { get; set; }
    }
}