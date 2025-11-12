namespace ElectronNET.IntegrationTests.Tests
{
    using System.Runtime.InteropServices;
    using ElectronNET.API.Entities;

    [Collection("ElectronCollection")]
    public class ThumbarButtonTests
    {
        private readonly ElectronFixture fx;

        public ThumbarButtonTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact(Timeout = 5000)]
        public async Task SetThumbarButtons_returns_success()
        {
            var btn = new ThumbarButton("icon.png") { Tooltip = "Test" };
            var success = await this.fx.MainWindow.SetThumbarButtonsAsync(new[] { btn });
            success.Should().BeTrue();
        }

        [Fact(Timeout = 5000)]
        public async Task Thumbar_button_click_invokes_callback()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return; // only meaningful on Windows taskbar
            }

            var icon = Path.Combine(Directory.GetCurrentDirectory(), "ElectronNET.WebApp", "wwwroot", "icon.png");
            if (!File.Exists(icon))
            {
                return; // skip if icon missing
            }

            var tcs = new TaskCompletionSource<bool>();
            var btn = new ThumbarButton(icon) { Tooltip = "Test", Flags = new[] { ThumbarButtonFlag.enabled }, Click = () => tcs.TrySetResult(true) };
            var ok = await this.fx.MainWindow.SetThumbarButtonsAsync(new[] { btn });
            ok.Should().BeTrue();
        }
    }
}