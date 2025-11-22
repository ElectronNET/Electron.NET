namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class Blob : IPostData
    {
        /// <summary>
        /// The object represents a Blob
        /// </summary>
        public string Type { get; } = "blob";

        /// <summary>
        /// The UUID of the Blob being uploaded
        /// </summary>
        public string BlobUUID { get; set; }
    }
}