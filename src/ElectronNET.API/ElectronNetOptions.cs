namespace ElectronNET
{
    /// <summary>
    /// Provides configuration options for Electron.NET. Consumers can assign
    /// an instance of <see cref="ElectronAppLifetimeEvents"/> to the <see cref="Events"/>
    /// property to hook into the Electron application lifecycle. Additional
    /// configuration properties can be added to this class in future versions without
    /// breaking existing consumers.
    /// </summary>
    public class ElectronNetOptions
    {
        /// <summary>
        /// Gets or sets the collection of lifecycle callbacks. The default value is
        /// an instance of <see cref="ElectronAppLifetimeEvents"/> with no-op
        /// implementations. Assigning a new instance or modifying individual
        /// callbacks allows consumers to customize the startup sequence.
        /// </summary>
        public ElectronAppLifetimeEvents Events { get; set; } = new ElectronAppLifetimeEvents();
    }
}