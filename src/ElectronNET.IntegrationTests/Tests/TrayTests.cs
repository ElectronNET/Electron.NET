namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;

    [Collection("ElectronCollection")]
    public class TrayTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ElectronFixture fx;
        public TrayTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact(Timeout = 5000)]
        public async Task Can_create_tray_and_destroy()
        {
            //await Electron.Tray.Show("assets/icon.png");
            await Electron.Tray.Show(null);
            var isDestroyed = await Electron.Tray.IsDestroyedAsync();
            isDestroyed.Should().BeFalse();
            await Electron.Tray.Destroy();
            (await Electron.Tray.IsDestroyedAsync()).Should().BeTrue();
        }
    }
}