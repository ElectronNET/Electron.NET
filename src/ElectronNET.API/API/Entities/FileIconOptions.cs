namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class FileIconOptions
    {
        /// <summary>
        /// The requested icon size string passed to app.getFileIcon:
        /// "small" (16x16), "normal" (32x32), or "large" (48x48 on Linux, 32x32 on Windows; unsupported on macOS).
        /// </summary>
        public string Size { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIconOptions"/> class.
        /// </summary>
        /// <param name="fileIconSize">Size of the file icon.</param>
        public FileIconOptions(FileIconSize fileIconSize)
        {
            Size = fileIconSize.ToString();
        }
    }
}