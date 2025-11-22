namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for BrowserWindow.loadURL(url, options) / webContents.loadURL(url, options).
    /// Matches Electron's loadURL options.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class LoadURLOptions
    {
        /// <summary>
        /// An HTTP Referrer URL. In Electron this may be a string or a Referrer object.
        /// </summary>
        public string HttpReferrer { get; set; }

        /// <summary>
        /// A user agent originating the request.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Base URL (with trailing path separator) for files to be loaded by the data URL.
        /// Needed only if the specified URL is a data URL and needs to load other files.
        /// </summary>
        public string BaseURLForDataURL { get; set; }

        /// <summary>
        /// Extra headers separated by "\n".
        /// </summary>
        public string ExtraHeaders { get; set; }

        /// <summary>
        /// Post data for the request. Matches Electron's postData: (UploadRawData | UploadFile)[]
        /// </summary>
        public IPostData[] PostData { get; set; }
    }
}