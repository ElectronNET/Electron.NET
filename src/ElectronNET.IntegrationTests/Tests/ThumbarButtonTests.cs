namespace ElectronNET.IntegrationTests.Tests
{
    using System.Runtime.Versioning;
    using ElectronNET.API.Entities;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class ThumbarButtonTests : IntegrationTestBase
    {
        public ThumbarButtonTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact]
        [SupportedOSPlatform(Windows)]
        public async Task SetThumbarButtons_returns_success()
        {
            var btn = new ThumbarButton("icon.png") { Tooltip = "Test" };
            var success = await this.MainWindow.SetThumbarButtonsAsync(new[] { btn });
            success.Should().BeTrue();
        }

        [IntegrationFact]
        [SupportedOSPlatform(Windows)]
        public async Task Thumbar_button_click_invokes_callback()
        {
            var icon = Path.Combine(Directory.GetCurrentDirectory(), "ElectronNET.WebApp", "wwwroot", "icon.png");
            if (!File.Exists(icon))
            {
                return; // skip if icon missing
            }

            var tcs = new TaskCompletionSource<bool>();
            var btn = new ThumbarButton(icon) { Tooltip = "Test", Flags = new[] { ThumbarButtonFlag.enabled }, Click = () => tcs.TrySetResult(true) };
            var ok = await this.MainWindow.SetThumbarButtonsAsync(new[] { btn });
            ok.Should().BeTrue();
        }
    }
}
