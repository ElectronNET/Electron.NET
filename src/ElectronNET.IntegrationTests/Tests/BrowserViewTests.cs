namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;

    [Collection("ElectronCollection")]
    public class BrowserViewTests
    {
        private readonly ElectronFixture fx;

        public BrowserViewTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact(Timeout = 20000)]
        public async Task Create_browser_view_and_adjust_bounds()
        {
            var view = await Electron.WindowManager.CreateBrowserViewAsync(new BrowserViewConstructorOptions());
            this.fx.MainWindow.SetBrowserView(view);
            view.Bounds = new Rectangle { X = 0, Y = 0, Width = 300, Height = 200 };
            // Access bounds again (synchronous property fetch)
            var current = view.Bounds;
            current.Width.Should().Be(300);
            current.Height.Should().Be(200);
        }
    }
}