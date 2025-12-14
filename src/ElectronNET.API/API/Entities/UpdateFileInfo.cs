namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with electron-updater 6.7.2</remarks>
    public class UpdateFileInfo : BlockMapDataHolder
    {
        /// <summary>
        /// Gets or sets the URL of the update file.
        /// </summary>
        public string Url { get; set; }
    }
}