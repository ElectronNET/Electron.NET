namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadRawData : IPostData
    {
        /// <summary>
        /// The data is available as a Buffer, in the rawData field.
        /// </summary>
        public string Type { get; } = "rawData";
        
        /// <summary>
        /// The raw bytes of the post data in a Buffer.
        /// </summary>
        public byte[] Bytes { get; set; }
    }
}