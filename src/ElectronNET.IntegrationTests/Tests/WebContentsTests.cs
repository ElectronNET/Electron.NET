using System.Runtime.InteropServices;

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

        [Fact(Timeout = 20000)]
        public async Task Can_get_url_after_navigation()
        {
            var wc = this.fx.MainWindow.WebContents;
            await wc.LoadURLAsync("https://example.com");
            var url = await wc.GetUrl();
            url.Should().Contain("example.com");
        }

        [Fact(Timeout = 20000)]
        public async Task ExecuteJavaScript_returns_title()
        {
            var wc = this.fx.MainWindow.WebContents;
            await wc.LoadURLAsync("https://example.com");
            var title = await wc.ExecuteJavaScriptAsync<string>("document.title");
            title.Should().NotBeNull();
        }

        [Fact(Timeout = 20000)]
        public async Task DomReady_event_fires()
        {
            var wc = this.fx.MainWindow.WebContents;
            var fired = false;
            wc.OnDomReady += () => fired = true;
            await wc.LoadURLAsync("https://example.com");
            await Task.Delay(500);
            fired.Should().BeTrue();
        }

        [Fact(Timeout = 20000)]
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

        [Fact(Timeout = 20000)]
        public async Task Can_basic_print()
        {
            var html = "data:text/html,<html><body><h2>Print Test</h2></body></html>";
            await this.fx.MainWindow.WebContents.LoadURLAsync(html);
            var ok = await this.fx.MainWindow.WebContents.PrintAsync(new PrintOptions { Silent = true, PrintBackground = true });
            ok.Should().BeTrue();
        }

        [SkippableFact(Timeout = 20000)]
        public async Task GetPrintersAsync_check()
        {
            Skip.If(Environment.GetEnvironmentVariable("GITHUB_TOKEN") != null, "Skipping printer test in CI environment.");
            var info = await fx.MainWindow.WebContents.GetPrintersAsync();
            info.Should().NotBeNull();
        }

        [Fact(Timeout = 20000)]
        public async Task GetSetZoomFactor_check()
        {
            await fx.MainWindow.WebContents.GetZoomFactorAsync();
            var ok = await fx.MainWindow.WebContents.GetZoomFactorAsync();
            ok.Should().BeGreaterThan(0.0);
            fx.MainWindow.WebContents.SetZoomFactor(2.0);
            await Task.Delay(500);
            ok = await fx.MainWindow.WebContents.GetZoomFactorAsync();
            ok.Should().Be(2.0);
        }

        [Fact(Timeout = 20000)]
        public async Task ZoomFactorProperty_check()
        {
            var ok =  fx.MainWindow.WebContents.ZoomFactor;
            ok.Should().Be(1.0);
            fx.MainWindow.WebContents.ZoomFactor = 2.0;
            ok = fx.MainWindow.WebContents.ZoomFactor;
            ok.Should().Be(2.0);
        }

        [Fact(Timeout = 20000)]
        public async Task GetSetZoomLevel_check()
        {
            await fx.MainWindow.WebContents.GetZoomLevelAsync();
            var ok = await fx.MainWindow.WebContents.GetZoomLevelAsync();
            ok.Should().Be(0);
            fx.MainWindow.WebContents.SetZoomLevel(2);
            await Task.Delay(500);
            ok = await fx.MainWindow.WebContents.GetZoomLevelAsync();
            ok.Should().Be(2);
        }

        [Fact(Timeout = 20000)]
        public async Task ZoomLevelProperty_check()
        {
            var ok =  fx.MainWindow.WebContents.ZoomLevel;
            ok.Should().Be(0);
            fx.MainWindow.WebContents.ZoomLevel = 2;
            ok = fx.MainWindow.WebContents.ZoomLevel;
            ok.Should().Be(2);
        }

        [SkippableFact(Timeout = 20000)]
        public async Task DevTools_check()
        {
            Skip.If(Environment.GetEnvironmentVariable("GITHUB_TOKEN") != null, "Skipping test in CI environment.");
            fx.MainWindow.WebContents.IsDevToolsOpened().Should().BeFalse();
            fx.MainWindow.WebContents.OpenDevTools();
            await Task.Delay(1000);
            fx.MainWindow.WebContents.IsDevToolsOpened().Should().BeTrue();
            fx.MainWindow.WebContents.CloseDevTools();
            await Task.Delay(1000);
            fx.MainWindow.WebContents.IsDevToolsOpened().Should().BeFalse();
            fx.MainWindow.WebContents.ToggleDevTools();
            await Task.Delay(1000);
            fx.MainWindow.WebContents.IsDevToolsOpened().Should().BeTrue();
        }

        [Fact(Timeout = 20000)]
        public async Task GetSetAudioMuted_check()
        {
            fx.MainWindow.WebContents.SetAudioMuted(true);
            var ok = await fx.MainWindow.WebContents.IsAudioMutedAsync();
            ok.Should().BeTrue();
            fx.MainWindow.WebContents.SetAudioMuted(false);
            ok = await fx.MainWindow.WebContents.IsAudioMutedAsync();
            ok.Should().BeFalse();

            // Assuming no audio is playing, IsCurrentlyAudibleAsync should return false
            // there is no way to play audio in this test
            ok = await fx.MainWindow.WebContents.IsCurrentlyAudibleAsync();
            ok.Should().BeFalse();
        }

        [SkippableFact(Timeout = 20000)]
        public async Task AudioMutedProperty_check()
        {
            Skip.If(Environment.GetEnvironmentVariable("GITHUB_TOKEN") != null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows), "Skipping test on Windows CI.");
            fx.MainWindow.WebContents.AudioMuted.Should().BeFalse();
            fx.MainWindow.WebContents.AudioMuted = true;
            fx.MainWindow.WebContents.AudioMuted.Should().BeTrue();
        }

        [Fact(Timeout = 20000)]
        public async Task GetSetUserAgent_check()
        {
            var ok = await fx.MainWindow.WebContents.GetUserAgentAsync();
            ok.Should().NotBeNullOrEmpty();
            fx.MainWindow.WebContents.SetUserAgent("MyUserAgent/1.0");
            await Task.Delay(1000);
            ok = await fx.MainWindow.WebContents.GetUserAgentAsync();
            ok.Should().Be("MyUserAgent/1.0");
        }

        [Fact(Timeout = 20000)]
        public async Task UserAgentProperty_check()
        {
            fx.MainWindow.WebContents.UserAgent.Should().NotBeNullOrEmpty();
            fx.MainWindow.WebContents.UserAgent = "MyUserAgent/1.0";
            fx.MainWindow.WebContents.UserAgent.Should().Be("MyUserAgent/1.0");
        }

    }
}