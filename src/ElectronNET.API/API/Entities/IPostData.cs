namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Represents a postData item for loadURL/webContents.loadURL options.
    /// Valid types per Electron docs: 'rawData' and 'file'.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public interface IPostData
    {
        /// <summary>
        /// One of the following:
        /// rawData - <see cref="UploadRawData"/>.
        /// file - <see cref="UploadFile"/>.
        /// Based on Electron postData definitions.
        /// </summary>
        public string Type { get; }
    }
}