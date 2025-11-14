namespace ElectronNET.IntegrationTests
{
    using System.Reflection;
    using ElectronNET.API;
    using ElectronNET.API.Entities;

    // Shared fixture that starts Electron runtime once
    public class ElectronFixture : IAsyncLifetime
    {
        public BrowserWindow MainWindow { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            AppDomain.CurrentDomain.SetData("ElectronTestAssembly", Assembly.GetExecutingAssembly());
            var runtimeController = ElectronNetRuntime.RuntimeController;
            await runtimeController.Start();
            await runtimeController.WaitReadyTask;

            // create hidden window for tests (avoid showing UI)
            this.MainWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Show = false,
                Width = 800,
                Height = 600,
            }, "about:blank");

            // Clear potential cache side-effects
            await this.MainWindow.WebContents.Session.ClearCacheAsync();
        }

        public async Task DisposeAsync()
        {
            var runtimeController = ElectronNetRuntime.RuntimeController;
            await runtimeController.Stop();
            await runtimeController.WaitStoppedTask;
        }
    }

    [CollectionDefinition("ElectronCollection")]
    public class ElectronCollection : ICollectionFixture<ElectronFixture>
    {
    }
}