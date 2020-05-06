namespace ElectronNET.API.Entities
{
    public class CreateFromBufferOptions
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public float ScaleFactor { get; set; } = 1.0f;
    }
}