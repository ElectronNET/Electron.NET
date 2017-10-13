namespace ElectronNET.API.Entities
{
    public class FileIconOptions
    {
        public string Size { get; private set; }

        public FileIconOptions(FileIconSize fileIconSize)
        {
            Size = fileIconSize.ToString();
        }
    }
}