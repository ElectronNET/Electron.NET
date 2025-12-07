namespace ElectronNET.IntegrationTests.Common
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;

    // Base class for integration tests providing shared access to MainWindow and OS platform constants
    public abstract class IntegrationTestBase
    {
        protected IntegrationTestBase(ElectronFixture fixture)
        {
            Fixture = fixture;
            MainWindow = fixture.MainWindow;
        }

        protected ElectronFixture Fixture { get; }
        protected BrowserWindow MainWindow { get; }

        // Constants for SupportedOSPlatform attributes
        public const string Windows = "Windows";
        public const string MacOS = "macOS";
        public const string Linux = "Linux";
    }
}
