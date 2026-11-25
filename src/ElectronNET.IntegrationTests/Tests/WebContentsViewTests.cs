namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class WebContentsViewTests : IntegrationTestBase
    {
        public WebContentsViewTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact]
        public async Task Create_web_contents_view_and_adjust_bounds()
        {
            var view = await Electron.WindowManager.CreateWebContentsViewAsync(new WebContentsViewConstructorOptions());
            this.MainWindow.AddChildView(view);
            view.SetBounds(new Rectangle { X = 0, Y = 0, Width = 300, Height = 200 });

            var current = await view.GetBoundsAsync();
            current.Width.Should().Be(300);
            current.Height.Should().Be(200);

            this.MainWindow.RemoveChildView(view);
        }

        [IntegrationFact]
        public async Task Web_contents_view_has_its_own_web_contents()
        {
            var view = await Electron.WindowManager.CreateWebContentsViewAsync();
            this.MainWindow.AddChildView(view);
            view.SetBounds(new Rectangle { X = 0, Y = 0, Width = 300, Height = 200 });

            await view.WebContents.LoadURLAsync("about:blank");
            var url = await view.WebContents.GetUrl();
            url.Should().Be("about:blank");

            this.MainWindow.RemoveChildView(view);
        }
    }
}
