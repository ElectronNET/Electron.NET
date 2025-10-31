namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateInterruptedDownloadOptions
    {
        /// <summary>
        /// Absolute path of the download.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Complete URL chain for the download.
        /// </summary>
        public string[] UrlChain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Start range for the download.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Total length of the download.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Last-Modified header value.
        /// </summary>
        public string LastModified { get; set; }

        /// <summary>
        /// ETag header value.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Time when download was started in number of seconds since UNIX epoch.
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Absolute path of the download.</param>
        /// <param name="urlChain">Complete URL chain for the download.</param>
        /// <param name="offset">Start range for the download.</param>
        /// <param name="length">Total length of the download.</param>
        /// <param name="lastModified">Last-Modified header value.</param>
        /// <param name="eTag">ETag header value.</param>
        public CreateInterruptedDownloadOptions(string path, string[] urlChain, int offset, int length, string lastModified, string eTag)
        {
            Path = path;
            UrlChain = urlChain;
            Offset = offset;
            Length = length;
            LastModified = lastModified;
            ETag = eTag;
        }
    }
}
