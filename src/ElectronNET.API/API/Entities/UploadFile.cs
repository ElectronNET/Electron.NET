namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadFile : IPostData
    {
        /// <summary>
        /// The object represents a file.
        /// </summary>
        public string Type { get; } = "file";

        /// <summary>
        /// The path of the file being uploaded.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The offset from the beginning of the file being uploaded, in bytes. Defaults to 0.
        /// </summary>
        public long Offset { get; set; } = 0;

        /// <summary>
        /// The length of the file being uploaded, <see cref="Offset"/>. Defaults to 0.
        /// If set to -1, the whole file will be uploaded.
        /// </summary>
        public long Length { get; set; } = 0;

        /// <summary>
        /// The modification time of the file represented by a double, which is the number of seconds since the UNIX Epoch (Jan 1, 1970)
        /// </summary>
        public double ModificationTime { get; set; }
    }
}