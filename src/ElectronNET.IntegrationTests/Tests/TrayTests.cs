namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class TrayTests : IntegrationTestBase
    {
        public TrayTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact]
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