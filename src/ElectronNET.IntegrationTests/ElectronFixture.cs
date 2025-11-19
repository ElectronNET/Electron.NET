namespace ElectronNET.IntegrationTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using ElectronNET.Runtime;

    // Shared fixture that starts Electron runtime once
    [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
    public class ElectronFixture : IAsyncLifetime
    {
        public BrowserWindow MainWindow { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            try
            {
                Console.Error.WriteLine("[ElectronFixture] InitializeAsync: start");
                AppDomain.CurrentDomain.SetData("ElectronTestAssembly", Assembly.GetExecutingAssembly());

                Console.WriteLine("[ElectronFixture] Acquire RuntimeController");
                var host = ElectronHostEnvironment.Current;
                var runtimeController = host.RuntimeController;
                host.ElectronExtraArguments = "--no-sandbox";

                Console.Error.WriteLine("[ElectronFixture] Starting Electron runtime...");
                await runtimeController.Start();

                Console.Error.WriteLine("[ElectronFixture] Waiting for Ready...");
                await Task.WhenAny(runtimeController.WaitReadyTask, Task.Delay(TimeSpan.FromSeconds(10)));

                if (!runtimeController.WaitReadyTask.IsCompleted)
                {
                    throw new TimeoutException("The Electron process did not start within 10 seconds");
                }

                Console.Error.WriteLine("[ElectronFixture] Runtime Ready");

                // create hidden window for tests (avoid showing UI)
                this.MainWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
                {
                    Show = false,
                    Width = 800,
                    Height = 600,
                }, "about:blank");

                await this.MainWindow.WebContents.Session.ClearCacheAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[ElectronFixture] InitializeAsync: exception");
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task DisposeAsync()
        {
            var runtimeController = ElectronHostEnvironment.Current.RuntimeController;
            Console.Error.WriteLine("[ElectronFixture] Stopping Electron runtime...");
            await runtimeController.Stop();
            await runtimeController.WaitStoppedTask;
            Console.Error.WriteLine("[ElectronFixture] Runtime stopped");
        }
    }

    [CollectionDefinition("ElectronCollection")]
    public class ElectronCollection : ICollectionFixture<ElectronFixture>
    {
    }
}