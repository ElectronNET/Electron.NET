namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Information about a download handled by a session.
    /// </summary>
    public class DownloadInfo
    {
        /// <summary>
        /// The origin URL of the download.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The file name suggested for the download.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The MIME type of the download.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The total size of the download in bytes, or 0 if unknown.
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// The path the download is saved to. Empty if it is not known yet, e.g. when the
        /// user still has to pick a location in the save dialog.
        /// </summary>
        public string SavePath { get; set; }
    }

    /// <summary>
    /// Information about a finished download.
    /// </summary>
    public class DownloadDoneInfo : DownloadInfo
    {
        /// <summary>
        /// How the download ended.
        /// </summary>
        public DownloadDoneState State { get; set; }

        /// <summary>
        /// The number of bytes received.
        /// </summary>
        public long ReceivedBytes { get; set; }
    }

    /// <summary>
    /// How a download ended (per the DownloadItem 'done' event).
    /// </summary>
    public enum DownloadDoneState
    {
        /// <summary>
        /// The download finished successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// The download was cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The download was interrupted and cannot be resumed.
        /// </summary>
        Interrupted,
    }
}
