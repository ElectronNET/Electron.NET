namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class FileIconOptions
    {
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
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