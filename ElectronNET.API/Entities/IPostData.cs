namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Interface to use Electrons PostData Object
    /// </summary>
    public interface IPostData
    {
        /// <summary>
        /// One of the following:
        /// rawData - <see cref="UploadRawData"/> The data is available as a Buffer, in the rawData field.
        /// file - <see cref="UploadFile"/> The object represents a file. The filePath, offset, length and modificationTime fields will be used to describe the file.
        /// blob - <see cref="Blob"/> The object represents a Blob. The blobUUID field will be used to describe the Blob.
        /// </summary>
        public string Type { get; }
    }
}