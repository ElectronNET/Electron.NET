using System.Runtime.InteropServices;

namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using ElectronNET.Common;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class WebContentsTests
    {
        private readonly ElectronFixture fx;

        public WebContentsTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [IntegrationFact]
        public async Task Can_get_url_after_navigation()
        {
            var wc = this.fx.MainWindow.WebContents;
            await wc.LoadURLAsync("https://example.com");
            var url = await wc.GetUrl();
            url.Should().Contain("example.com");
        }

        [IntegrationFact]
        public async Task ExecuteJavaScript_returns_title()
        {
            var wc = this.fx.MainWindow.WebContents;
            await wc.LoadURLAsync("https://example.com");
            var title = await wc.ExecuteJavaScriptAsync<string>("document.title");
            title.Should().NotBeNull();
        }

        [IntegrationFact]
        public async Task DomReady_event_fires()
        {
            var wc = this.fx.MainWindow.WebContents;
            var fired = false;
            wc.OnDomReady += () => fired = true;
            await wc.LoadURLAsync("https://example.com");
            await Task.Delay(500.ms());
            fired.Should().BeTrue();
        }

        [IntegrationFact]
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

        [IntegrationFact]
        public async Task Can_basic_print()
        {
            var html = "data:text/html,<html><body><h2>Print Test</h2></body></html>";
            await this.fx.MainWindow.WebContents.LoadURLAsync(html);
            var ok = await this.fx.MainWindow.WebContents.PrintAsync(new PrintOptions { Silent = true, PrintBackground = true });
            ok.Should().BeTrue();
        }

        [IntegrationFact]
        public async Task GetPrintersAsync_check()
        {
            var info = await fx.MainWindow.WebContents.GetPrintersAsync();
            info.Should().NotBeNull();
        }

        [IntegrationFact]
        public async Task GetSetZoomFactor_check()
        {
            await fx.MainWindow.WebContents.GetZoomFactorAsync();
            var ok = await fx.MainWindow.WebContents.GetZoomFactorAsync();
            ok.Should().BeGreaterThan(0.0);
            fx.MainWindow.WebContents.SetZoomFactor(2.0);
            await Task.Delay(500.ms());
            ok = await fx.MainWindow.WebContents.GetZoomFactorAsync();
            ok.Should().Be(2.0);
        }

        [IntegrationFact]
        public async Task GetSetZoomLevel_check()
        {
            BrowserWindow window = null;

            try
            {
                window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = true }, "about:blank");

                await Task.Delay(100.ms());

                window.WebContents.SetZoomLevel(0);
                await Task.Delay(500.ms());

                var ok = await window.WebContents.GetZoomLevelAsync();
                ok.Should().Be(0);

                window.WebContents.SetZoomLevel(2);
                await Task.Delay(500.ms());

                ok = await window.WebContents.GetZoomLevelAsync();
                ok.Should().Be(2);
            }
            finally
            {
                window?.Destroy();
            }
        }

        [IntegrationFact]
        public async Task DevTools_check()
        {
            BrowserWindow window = null;

            try
            {
                window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = true }, "about:blank");

                await Task.Delay(3.seconds());

                window.WebContents.IsDevToolsOpened().Should().BeFalse();
                window.WebContents.OpenDevTools();
                await Task.Delay(5.seconds());

                window.WebContents.IsDevToolsOpened().Should().BeTrue();
                window.WebContents.CloseDevTools();
                await Task.Delay(2.seconds());

                window.WebContents.IsDevToolsOpened().Should().BeFalse();
            }
            finally
            {
                window?.Destroy();
            }
        }

        [IntegrationFact]
        public async Task GetSetAudioMuted_check()
        {
            fx.MainWindow.WebContents.SetAudioMuted(true);
            await Task.Delay(500.ms());
            var ok = await fx.MainWindow.WebContents.IsAudioMutedAsync();
            ok.Should().BeTrue();
            fx.MainWindow.WebContents.SetAudioMuted(false);
            await Task.Delay(500.ms());
            ok = await fx.MainWindow.WebContents.IsAudioMutedAsync();
            ok.Should().BeFalse();

            // Assuming no audio is playing, IsCurrentlyAudibleAsync should return false
            // there is no way to play audio in this test
            ok = await fx.MainWindow.WebContents.IsCurrentlyAudibleAsync();
            ok.Should().BeFalse();
        }

        [IntegrationFact]
        public async Task GetSetUserAgent_check()
        {
            BrowserWindow window = null;

            try
            {
                window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = true }, "about:blank");

                await Task.Delay(3.seconds());

                window.WebContents.SetUserAgent("MyUserAgent/1.0");

                await Task.Delay(1.seconds());

                var ok = await window.WebContents.GetUserAgentAsync();
                ok.Should().Be("MyUserAgent/1.0");
            }
            finally
            {
                window?.Destroy();
            }
        }

    }
}