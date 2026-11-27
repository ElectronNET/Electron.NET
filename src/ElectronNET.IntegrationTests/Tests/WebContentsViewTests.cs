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

        [IntegrationFact]
        public async Task Concurrent_bounds_requests_return_the_bounds_of_each_view()
        {
            var viewA = await Electron.WindowManager.CreateWebContentsViewAsync();
            var viewB = await Electron.WindowManager.CreateWebContentsViewAsync();

            try
            {
                this.MainWindow.AddChildView(viewA);
                this.MainWindow.AddChildView(viewB);
                viewA.SetBounds(new Rectangle { X = 0, Y = 0, Width = 300, Height = 200 });
                viewB.SetBounds(new Rectangle { X = 10, Y = 20, Width = 120, Height = 80 });

                var boundsA = viewA.GetBoundsAsync();
                var boundsB = viewB.GetBoundsAsync();
                await Task.WhenAll(boundsA, boundsB);

                boundsA.Result.Width.Should().Be(300);
                boundsA.Result.Height.Should().Be(200);
                boundsB.Result.Width.Should().Be(120);
                boundsB.Result.Height.Should().Be(80);
            }
            finally
            {
                await viewA.DestroyAsync();
                await viewB.DestroyAsync();
            }
        }

        [IntegrationFact]
        public async Task Removed_web_contents_view_can_be_reused()
        {
            var view = await Electron.WindowManager.CreateWebContentsViewAsync();

            try
            {
                this.MainWindow.AddChildView(view);
                this.MainWindow.RemoveChildView(view);
                this.MainWindow.AddChildView(view);
                view.SetBounds(new Rectangle { X = 0, Y = 0, Width = 150, Height = 100 });

                var bounds = await view.GetBoundsAsync();
                bounds.Width.Should().Be(150);
                Electron.WindowManager.WebContentsViews.Should().Contain(view);
            }
            finally
            {
                await view.DestroyAsync();
            }
        }

        [IntegrationFact]
        public async Task Destroyed_web_contents_view_is_released()
        {
            var view = await Electron.WindowManager.CreateWebContentsViewAsync();
            this.MainWindow.AddChildView(view);

            await view.DestroyAsync();

            view.IsDestroyed.Should().BeTrue();
            Electron.WindowManager.WebContentsViews.Should().NotContain(view);
            await FluentActions.Awaiting(() => view.GetBoundsAsync()).Should().ThrowAsync<ObjectDisposedException>();

            // Destroying twice is a no-op.
            await view.DestroyAsync();
        }
    }
}
