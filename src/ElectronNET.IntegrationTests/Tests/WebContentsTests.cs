namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API.Entities;

    [Collection("ElectronCollection")]
    public class WebContentsTests
    {
        private readonly ElectronFixture fx;

        public WebContentsTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact]
        public async Task Can_get_url_after_navigation()
        {
            var wc = this.fx.MainWindow.WebContents;
            await wc.LoadURLAsync("https://example.com");
            var url = await wc.GetUrl();
            url.Should().Contain("example.com");
        }

        [Fact]
        public async Task ExecuteJavaScript_returns_title()
        {
            var wc = this.fx.MainWindow.WebContents;
            await wc.LoadURLAsync("https://example.com");
            var title = await wc.ExecuteJavaScriptAsync<string>("document.title");
            title.Should().NotBeNull();
        }

        [Fact]
        public async Task DomReady_event_fires()
        {
            var wc = this.fx.MainWindow.WebContents;
            var fired = false;
            wc.OnDomReady += () => fired = true;
            await wc.LoadURLAsync("https://example.com");
            await Task.Delay(500);
            fired.Should().BeTrue();
        }

        [Fact]
        public async Task Can_print_to_pdf()
        {
            var html = "data:text/html,<html><body><h1>PDF Test</h1><p>Electron.NET</p></body></html>";
            await this.fx.MainWindow.WebContents.LoadURLAsync(html);
            var tmp = Path.Combine(Path.GetTempPath(), $"electronnet_pdf_{Guid.NewGuid():N}.pdf");
            try
            {
                var ok = await this.fx.MainWindow.WebContents.PrintToPDFAsync(tmp);
                ok.Should().BeTrue();
                File.Exists(tmp).Should().BeTrue();
                new FileInfo(tmp).Length.Should().BeGreaterThan(0);
            }
            finally
            {
                if (File.Exists(tmp))
                {
                    File.Delete(tmp);
                }
            }
        }

        [Fact]
        public async Task Can_basic_print()
        {
            var html = "data:text/html,<html><body><h2>Print Test</h2></body></html>";
            await this.fx.MainWindow.WebContents.LoadURLAsync(html);
            var ok = await this.fx.MainWindow.WebContents.PrintAsync(new PrintOptions { Silent = true, PrintBackground = true });
            ok.Should().BeTrue();
        }
        
        [Fact]
        public async Task GetPrintersAsync_check()
        {
            var info = await fx.MainWindow.WebContents.GetPrintersAsync();
            info.Should().NotBeNull();
        }
    }
}