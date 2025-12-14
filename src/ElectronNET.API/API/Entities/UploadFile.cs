namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class UploadFile : IPostData
    {
        /// <summary>
        /// Gets the type discriminator; constant 'file'.
        /// </summary>
        public string Type { get; } = "file";

        /// <summary>
        /// Gets or sets the path of the file to be uploaded.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the offset from the beginning of the file being uploaded, in bytes. Defaults to 0.
        /// </summary>
        public long Offset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of bytes to read from offset. Defaults to 0.
        /// </summary>
        public long Length { get; set; } = 0;

        /// <summary>
        /// Gets or sets the last modification time in number of seconds since the UNIX epoch. Defaults to 0.
        /// </summary>
        public double ModificationTime { get; set; } = 0;
    }
}