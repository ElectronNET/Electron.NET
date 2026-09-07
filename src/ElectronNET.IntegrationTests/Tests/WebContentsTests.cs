using System.Runtime.InteropServices;

namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using ElectronNET.Common;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class WebContentsTests : IntegrationTestBase
    {
        public WebContentsTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact]
        public async Task Can_get_url_after_navigation()
        {
            var wc = this.MainWindow.WebContents;
            await wc.LoadURLAsync("https://example.com");
            var url = await wc.GetUrl();
            url.Should().Contain("example.com");
        }

        [IntegrationFact]
        public async Task ExecuteJavaScript_returns_title()
        {
            var wc = this.MainWindow.WebContents;
            await wc.LoadURLAsync("https://example.com");
            var title = await wc.ExecuteJavaScriptAsync<string>("document.title");
            title.Should().NotBeNull();
        }

        [IntegrationFact]
        public async Task DomReady_event_fires()
        {
            var wc = this.MainWindow.WebContents;
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
            await this.MainWindow.WebContents.LoadURLAsync(html);
            var tmp = Path.Combine(Path.GetTempPath(), $"electronnet_pdf_{Guid.NewGuid():N}.pdf");
            try
            {
                var ok = await this.MainWindow.WebContents.PrintToPDFAsync(tmp);
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
            await this.MainWindow.WebContents.LoadURLAsync(html);
            var ok = await this.MainWindow.WebContents.PrintAsync(new PrintOptions { Silent = true, PrintBackground = true });
            ok.Should().BeTrue();
        }

        [IntegrationFact]
        public async Task GetPrintersAsync_check()
        {
            var info = await this.MainWindow.WebContents.GetPrintersAsync();
            info.Should().NotBeNull();
        }

        [IntegrationFact]
        public async Task GetSetZoomFactor_check()
        {
            await this.MainWindow.WebContents.GetZoomFactorAsync();
            var ok = await this.MainWindow.WebContents.GetZoomFactorAsync();
            ok.Should().BeGreaterThan(0.0);
            this.MainWindow.WebContents.SetZoomFactor(2.0);
            await Task.Delay(500.ms());
            ok = await this.MainWindow.WebContents.GetZoomFactorAsync();
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
                ok.Should().Be(0.0);

                window.WebContents.SetZoomLevel(2);
                await Task.Delay(500.ms());

                ok = await window.WebContents.GetZoomLevelAsync();
                ok.Should().Be(2.0);

                window.WebContents.SetZoomLevel(0.5);
                await Task.Delay(500.ms());

                ok = await window.WebContents.GetZoomLevelAsync();
                ok.Should().BeApproximately(0.5, 0.001);
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
            this.MainWindow.WebContents.SetAudioMuted(true);
            await Task.Delay(500.ms());
            var ok = await this.MainWindow.WebContents.IsAudioMutedAsync();
            ok.Should().BeTrue();
            this.MainWindow.WebContents.SetAudioMuted(false);
            await Task.Delay(500.ms());
            ok = await this.MainWindow.WebContents.IsAudioMutedAsync();
            ok.Should().BeFalse();

            // Assuming no audio is playing, IsCurrentlyAudibleAsync should return false
            // there is no way to play audio in this test
            ok = await this.MainWindow.WebContents.IsCurrentlyAudibleAsync();
            ok.Should().BeFalse();
        }

        [IntegrationFact]
        public async Task GetSetUserAgent_check()
        {
            BrowserWindow window = null;

            try
            {
                await Task.Delay(1.seconds());

                window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = true }, "about:blank");

                await Task.Delay(5.seconds());

                window.WebContents.SetUserAgent("MyUserAgent/1.0");

                await Task.Delay(2.seconds());

                var ok = await window.WebContents.GetUserAgentAsync();
                ok.Should().Be("MyUserAgent/1.0");
            }
            finally
            {
                window?.Destroy();
            }
        }

        [IntegrationFact]
        public async Task InsertAndRemoveCSS_check()
        {
            var wc = this.MainWindow.WebContents;
            await wc.LoadURLAsync("data:text/html,<html><body><p>CSS Test</p></body></html>");

            var key = await wc.InsertCSSAsync("body { background-color: rgb(255, 0, 0); }");
            key.Should().NotBeNullOrEmpty();

            var color = await wc.ExecuteJavaScriptAsync<string>("getComputedStyle(document.body).backgroundColor");
            color.Should().Be("rgb(255, 0, 0)");

            await wc.RemoveInsertedCSSAsync(key);

            color = await wc.ExecuteJavaScriptAsync<string>("getComputedStyle(document.body).backgroundColor");
            color.Should().NotBe("rgb(255, 0, 0)");
        }

        [IntegrationFact]
        public async Task EditCommands_check()
        {
            var wc = this.MainWindow.WebContents;
            await wc.LoadURLAsync("data:text/html,<html><body><input id='in' autofocus /></body></html>");
            await Task.Delay(500.ms());

            await wc.InsertTextAsync("Electron.NET");
            await Task.Delay(500.ms());

            var value = await wc.ExecuteJavaScriptAsync<string>("document.getElementById('in').value");
            value.Should().Be("Electron.NET");

            wc.SelectAll();
            wc.Delete();
            await Task.Delay(500.ms());

            value = await wc.ExecuteJavaScriptAsync<string>("document.getElementById('in').value");
            value.Should().BeEmpty();
        }

        [IntegrationFact]
        public async Task FindInPage_check()
        {
            var wc = this.MainWindow.WebContents;
            await wc.LoadURLAsync("data:text/html,<html><body><p>needle in a haystack</p></body></html>");
            await Task.Delay(500.ms());

            var tcs = new TaskCompletionSource<FoundInPageResult>();

            void OnFound(FoundInPageResult result) => tcs.TrySetResult(result);

            wc.OnFoundInPage += OnFound;

            try
            {
                var requestId = await wc.FindInPageAsync("needle");
                requestId.Should().BeGreaterThan(0);

                var completed = await Task.WhenAny(tcs.Task, Task.Delay(5.seconds()));
                completed.Should().Be(tcs.Task);

                var result = await tcs.Task;
                result.RequestId.Should().Be(requestId);
                result.Matches.Should().BeGreaterThan(0);
            }
            finally
            {
                wc.OnFoundInPage -= OnFound;
                wc.StopFindInPage(StopFindInPageAction.ClearSelection);
            }
        }

        [IntegrationFact]
        public async Task Reload_and_loading_state_check()
        {
            var wc = this.MainWindow.WebContents;
            await wc.LoadURLAsync("data:text/html,<html><body><h1>Reload Test</h1></body></html>");
            await Task.Delay(500.ms());

            (await wc.IsLoadingAsync()).Should().BeFalse();
            (await wc.IsLoadingMainFrameAsync()).Should().BeFalse();
            (await wc.IsWaitingForResponseAsync()).Should().BeFalse();

            wc.Reload();
            await Task.Delay(1.seconds());

            var title = await wc.ExecuteJavaScriptAsync<string>("document.querySelector('h1').textContent");
            title.Should().Be("Reload Test");
        }

    }
}