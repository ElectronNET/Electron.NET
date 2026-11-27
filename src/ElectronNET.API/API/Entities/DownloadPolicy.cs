namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Rules that decide how a session handles downloads. They are applied synchronously
    /// inside Electron's 'will-download' event, so a download can be blocked or stored
    /// without any round trip to .NET.
    /// </summary>
    public class DownloadPolicy
    {
        /// <summary>
        /// File extensions that are never downloaded, e.g. ".exe" or ".tar.gz". Matching is
        /// case-insensitive and the leading dot is optional. Takes precedence over
        /// <see cref="AllowedExtensions"/>.
        /// </summary>
        public string[] BlockedExtensions { get; set; }

        /// <summary>
        /// If set and not empty, only files with one of these extensions are downloaded;
        /// every other download is blocked. Matching is case-insensitive and the leading
        /// dot is optional.
        /// </summary>
        public string[] AllowedExtensions { get; set; }

        /// <summary>
        /// If set, allowed downloads are stored in this directory without showing a save dialog.
        /// The directory is created if it does not exist. If not set, Electron's default
        /// behavior (usually a save dialog) is used.
        /// </summary>
        public string SaveDirectory { get; set; }

        /// <summary>
        /// Only used together with <see cref="SaveDirectory"/>. If false (default), a numeric
        /// suffix is appended when a file with the same name already exists, e.g. "report (1).pdf".
        /// If true, the existing file is overwritten.
        /// </summary>
        public bool Overwrite { get; set; }
    }
}
