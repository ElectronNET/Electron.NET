namespace ElectronNET.IntegrationTests.Tests
{
    using System.Runtime.Versioning;
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using ElectronNET.Common;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class BrowserWindowTests : IntegrationTestBase
    {
        public BrowserWindowTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact]
        public async Task Can_set_and_get_title()
        {
            const string title = "Integration Test Title";
            this.MainWindow.SetTitle(title);
            await Task.Delay(500.ms());
            var roundTrip = await this.MainWindow.GetTitleAsync();
            roundTrip.Should().Be(title);
        }

        [IntegrationFact]
        public async Task Can_resize_and_get_size()
        {
            this.MainWindow.SetSize(643, 482);
            await Task.Delay(500.ms());
            var size = await this.MainWindow.GetSizeAsync();
            size.Should().HaveCount(2);
            size[0].Should().Be(643);
            size[1].Should().Be(482);
        }

        [IntegrationFact]
        public async Task Can_set_progress_bar_and_clear()
        {
            this.MainWindow.SetProgressBar(0.5);
            // No direct getter; rely on absence of error. Try changing again.
            this.MainWindow.SetProgressBar(-1); // clears
            await Task.Delay(50.ms());
        }

        [IntegrationFact(SkipOnWsl = true)]
        public async Task Can_set_and_get_position()
        {
            this.MainWindow.SetPosition(134, 246);
            await Task.Delay(500.ms());
            var pos = await this.MainWindow.GetPositionAsync();
            pos.Should().BeEquivalentTo([134, 246]);
        }

        [IntegrationFact]
        public async Task Can_set_and_get_bounds()
        {
            var bounds = new Rectangle { X = 10, Y = 20, Width = 400, Height = 300 };
            this.MainWindow.SetBounds(bounds);
            await Task.Delay(500.ms());
            var round = await this.MainWindow.GetBoundsAsync();

            round.Should().BeEquivalentTo(bounds);
            round.Width.Should().Be(400);
            round.Height.Should().Be(300);
        }

        [IntegrationFact]
        public async Task Can_set_and_get_content_bounds()
        {
            var bounds = new Rectangle { X = 0, Y = 0, Width = 300, Height = 200 };
            this.MainWindow.SetContentBounds(bounds);
            await Task.Delay(500.ms());
            var round = await this.MainWindow.GetContentBoundsAsync();
            round.Width.Should().BeGreaterThan(0);
            round.Height.Should().BeGreaterThan(0);
        }

        [IntegrationFact]
        public async Task Show_hide_visibility_roundtrip()
        {
            BrowserWindow window = null;

            try
            {
                window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = true }, "about:blank");

                await Task.Delay(100.ms());

                window.Show();

                await Task.Delay(500.ms());
                (await window.IsVisibleAsync()).Should().BeTrue();

                window.Hide();
                await Task.Delay(500.ms());

                (await window.IsVisibleAsync()).Should().BeFalse();
            }
            finally
            {
                window?.Destroy();
            }
        }

        [IntegrationFact]
        public async Task AlwaysOnTop_toggle_and_query()
        {
            this.MainWindow.SetAlwaysOnTop(true);
            await Task.Delay(500.ms());
            (await this.MainWindow.IsAlwaysOnTopAsync()).Should().BeTrue();
            this.MainWindow.SetAlwaysOnTop(false);
            await Task.Delay(500.ms());
            (await this.MainWindow.IsAlwaysOnTopAsync()).Should().BeFalse();
        }

        [IntegrationFact]
        [SupportedOSPlatform(Linux)]
        [SupportedOSPlatform(Windows)]
        public async Task MenuBar_auto_hide_and_visibility()
        {
            this.MainWindow.SetAutoHideMenuBar(true);
            await Task.Delay(500.ms());
            (await this.MainWindow.IsMenuBarAutoHideAsync()).Should().BeTrue();
            this.MainWindow.SetMenuBarVisibility(false);
            await Task.Delay(500.ms());
            (await this.MainWindow.IsMenuBarVisibleAsync()).Should().BeFalse();
            this.MainWindow.SetMenuBarVisibility(true);
            await Task.Delay(500.ms());
            (await this.MainWindow.IsMenuBarVisibleAsync()).Should().BeTrue();
        }

        [IntegrationFact]
        public async Task ReadyToShow_event_fires_after_content_ready()
        {
            BrowserWindow window = null;

            try
            {
                window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = false }, "about:blank");
                var tcs = new TaskCompletionSource();
                window.OnReadyToShow += () => tcs.TrySetResult();

                // Trigger a navigation and wait for DOM ready so the renderer paints, which emits ready-to-show
                var domReadyTcs = new TaskCompletionSource();
                window.WebContents.OnDomReady += () => domReadyTcs.TrySetResult();
                await Task.Delay(500.ms());
                await window.WebContents.LoadURLAsync("about:blank");
                await domReadyTcs.Task;

                var completed = await Task.WhenAny(tcs.Task, Task.Delay(3.seconds()));
                completed.Should().Be(tcs.Task);
            }
            finally
            {
                window?.Destroy();
            }
        }

        [IntegrationFact]
        public async Task PageTitleUpdated_event_fires_on_title_change()
        {
            var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = true }, "about:blank");
            var tcs = new TaskCompletionSource<string>();
            window.OnPageTitleUpdated += title => tcs.TrySetResult(title);

            // Navigate and wait for DOM ready, then change the document.title to trigger the event
            var domReadyTcs = new TaskCompletionSource();
            window.WebContents.OnDomReady += () => domReadyTcs.TrySetResult();
            await Task.Delay(500.ms());
            await window.WebContents.LoadURLAsync("about:blank");
            await domReadyTcs.Task;
            await window.WebContents.ExecuteJavaScriptAsync<string>("document.title='NewTitle';");

            // Wait for event up to a short timeout
            var completed2 = await Task.WhenAny(tcs.Task, Task.Delay(3.seconds()));
            completed2.Should().Be(tcs.Task);
            (await tcs.Task).Should().Be("NewTitle");
        }

        [IntegrationFact]
        public async Task Resize_event_fires_on_size_change()
        {
            var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = false }, "about:blank");
            var resized = false;
            window.OnResize += () => resized = true;
            await Task.Delay(500.ms());
            window.SetSize(500, 400);
            await Task.Delay(300.ms());
            resized.Should().BeTrue();
        }

        [IntegrationFact]
        public async Task Progress_bar_and_always_on_top_toggle()
        {
            var win = this.MainWindow;
            win.SetProgressBar(0.5);
            await Task.Delay(50.ms());
            win.SetProgressBar(0.8, new ProgressBarOptions());
            await Task.Delay(50.ms());
            win.SetAlwaysOnTop(true);
            await Task.Delay(500.ms());
            (await win.IsAlwaysOnTopAsync()).Should().BeTrue();
            win.SetAlwaysOnTop(false);
            await Task.Delay(500.ms());
            (await win.IsAlwaysOnTopAsync()).Should().BeFalse();
        }

        [IntegrationFact]
        [SupportedOSPlatform(Linux)]
        [SupportedOSPlatform(Windows)]
        public async Task Menu_bar_visibility_and_auto_hide()
        {
            var win = this.MainWindow;
            win.SetAutoHideMenuBar(true);
            await Task.Delay(500.ms());
            (await win.IsMenuBarAutoHideAsync()).Should().BeTrue();
            win.SetMenuBarVisibility(true);
            await Task.Delay(500.ms());
            (await win.IsMenuBarVisibleAsync()).Should().BeTrue();
        }

        [IntegrationFact]
        public async Task Parent_child_relationship_roundtrip()
        {
            var child = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = false, Width = 300, Height = 200 }, "about:blank");
            this.MainWindow.SetParentWindow(null); // ensure top-level
            child.SetParentWindow(this.MainWindow);
            await Task.Delay(500.ms());
            var parent = await child.GetParentWindowAsync();
            parent.Id.Should().Be(this.MainWindow.Id);
            var kids = await this.MainWindow.GetChildWindowsAsync();
            kids.Select(k => k.Id).Should().Contain(child.Id);
            child.Destroy();
        }

        [IntegrationFact]
        [SupportedOSPlatform(MacOS)]
        public async Task Represented_filename_and_edited_flags()
        {
            var win = this.MainWindow;
            var temp = Path.Combine(Path.GetTempPath(), "electronnet_test.txt");
            File.WriteAllText(temp, "test");
            win.SetRepresentedFilename(temp);

            await Task.Delay(500.ms());

            var represented = await win.GetRepresentedFilenameAsync();
            represented.Should().Be(temp);

            win.SetDocumentEdited(true);

            await Task.Delay(500.ms());

            var edited = await win.IsDocumentEditedAsync();
            edited.Should().BeTrue();

            win.SetDocumentEdited(false);
        }
    }
}
