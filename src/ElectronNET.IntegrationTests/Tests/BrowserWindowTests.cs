namespace ElectronNET.IntegrationTests.Tests
{
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using ElectronNET.API;
    using ElectronNET.API.Entities;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class BrowserWindowTests
    {
        private readonly ElectronFixture fx;

        public BrowserWindowTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact(Timeout = 20000)]
        public async Task Can_set_and_get_title()
        {
            const string title = "Integration Test Title";
            this.fx.MainWindow.SetTitle(title);
            await Task.Delay(500);
            var roundTrip = await this.fx.MainWindow.GetTitleAsync();
            roundTrip.Should().Be(title);
        }

        [Fact(Timeout = 20000)]
        public async Task Can_resize_and_get_size()
        {
            this.fx.MainWindow.SetSize(643, 482);
            await Task.Delay(500);
            var size = await this.fx.MainWindow.GetSizeAsync();
            size.Should().HaveCount(2);
            size[0].Should().Be(643);
            size[1].Should().Be(482);
        }

        [Fact(Timeout = 20000)]
        public async Task Can_set_progress_bar_and_clear()
        {
            this.fx.MainWindow.SetProgressBar(0.5);
            // No direct getter; rely on absence of error. Try changing again.
            this.fx.MainWindow.SetProgressBar(-1); // clears
            await Task.Delay(50);
        }

        [SkipOnWslFact(Timeout = 20000)]
        public async Task Can_set_and_get_position()
        {
            this.fx.MainWindow.SetPosition(134, 246);
            await Task.Delay(500);
            var pos = await this.fx.MainWindow.GetPositionAsync();
            pos.Should().BeEquivalentTo([134, 246]);
        }

        [Fact(Timeout = 20000)]
        public async Task Can_set_and_get_bounds()
        {
            var bounds = new Rectangle { X = 10, Y = 20, Width = 400, Height = 300 };
            this.fx.MainWindow.SetBounds(bounds);
            await Task.Delay(500);
            var round = await this.fx.MainWindow.GetBoundsAsync();

            round.Should().BeEquivalentTo(bounds);
            round.Width.Should().Be(400);
            round.Height.Should().Be(300);
        }

        [Fact(Timeout = 20000)]
        public async Task Can_set_and_get_content_bounds()
        {
            var bounds = new Rectangle { X = 0, Y = 0, Width = 300, Height = 200 };
            this.fx.MainWindow.SetContentBounds(bounds);
            await Task.Delay(500);
            var round = await this.fx.MainWindow.GetContentBoundsAsync();
            round.Width.Should().BeGreaterThan(0);
            round.Height.Should().BeGreaterThan(0);
        }

        [Fact(Timeout = 20000)]
        public async Task Show_hide_visibility_roundtrip()
        {
            this.fx.MainWindow.Show();
            await Task.Delay(500);
            (await this.fx.MainWindow.IsVisibleAsync()).Should().BeTrue();
            this.fx.MainWindow.Hide();
            await Task.Delay(500);
            (await this.fx.MainWindow.IsVisibleAsync()).Should().BeFalse();
        }

        [Fact(Timeout = 20000)]
        public async Task AlwaysOnTop_toggle_and_query()
        {
            this.fx.MainWindow.SetAlwaysOnTop(true);
            await Task.Delay(500);
            (await this.fx.MainWindow.IsAlwaysOnTopAsync()).Should().BeTrue();
            this.fx.MainWindow.SetAlwaysOnTop(false);
            await Task.Delay(500);
            (await this.fx.MainWindow.IsAlwaysOnTopAsync()).Should().BeFalse();
        }

        [SkippableFact(Timeout = 20000)]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("Windows")]
        public async Task MenuBar_auto_hide_and_visibility()
        {
            this.fx.MainWindow.SetAutoHideMenuBar(true);
            await Task.Delay(500);
            (await this.fx.MainWindow.IsMenuBarAutoHideAsync()).Should().BeTrue();
            this.fx.MainWindow.SetMenuBarVisibility(false);
            await Task.Delay(500);
            (await this.fx.MainWindow.IsMenuBarVisibleAsync()).Should().BeFalse();
            this.fx.MainWindow.SetMenuBarVisibility(true);
            await Task.Delay(500);
            (await this.fx.MainWindow.IsMenuBarVisibleAsync()).Should().BeTrue();
        }

        [Fact(Timeout = 20000)]
        public async Task ReadyToShow_event_fires_after_content_ready()
        {
            var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = false }, "about:blank");
            var tcs = new TaskCompletionSource();
            window.OnReadyToShow += () => tcs.TrySetResult();

            // Trigger a navigation and wait for DOM ready so the renderer paints, which emits ready-to-show
            var domReadyTcs = new TaskCompletionSource();
            window.WebContents.OnDomReady += () => domReadyTcs.TrySetResult();
            await Task.Delay(500);
            await window.WebContents.LoadURLAsync("about:blank");
            await domReadyTcs.Task;

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(3000));
            completed.Should().Be(tcs.Task);

            // Typical usage is to show once ready
            window.Show();
        }

        [Fact(Timeout = 20000)]
        public async Task PageTitleUpdated_event_fires_on_title_change()
        {
            var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = true }, "about:blank");
            var tcs = new TaskCompletionSource<string>();
            window.OnPageTitleUpdated += title => tcs.TrySetResult(title);

            // Navigate and wait for DOM ready, then change the document.title to trigger the event
            var domReadyTcs = new TaskCompletionSource();
            window.WebContents.OnDomReady += () => domReadyTcs.TrySetResult();
            await Task.Delay(500);
            await window.WebContents.LoadURLAsync("about:blank");
            await domReadyTcs.Task;
            await window.WebContents.ExecuteJavaScriptAsync<string>("document.title='NewTitle';");

            // Wait for event up to a short timeout
            var completed2 = await Task.WhenAny(tcs.Task, Task.Delay(3000));
            completed2.Should().Be(tcs.Task);
            (await tcs.Task).Should().Be("NewTitle");
        }

        [Fact(Timeout = 20000)]
        public async Task Resize_event_fires_on_size_change()
        {
            var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = false }, "about:blank");
            var resized = false;
            window.OnResize += () => resized = true;
            await Task.Delay(500);
            window.SetSize(500, 400);
            await Task.Delay(300);
            resized.Should().BeTrue();
        }

        [Fact(Timeout = 20000)]
        public async Task Progress_bar_and_always_on_top_toggle()
        {
            var win = this.fx.MainWindow;
            win.SetProgressBar(0.5);
            await Task.Delay(50);
            win.SetProgressBar(0.8, new ProgressBarOptions { Mode = ProgressBarMode.normal });
            await Task.Delay(50);
            win.SetAlwaysOnTop(true);
            await Task.Delay(500);
            (await win.IsAlwaysOnTopAsync()).Should().BeTrue();
            win.SetAlwaysOnTop(false);
            await Task.Delay(500);
            (await win.IsAlwaysOnTopAsync()).Should().BeFalse();
        }

        [SkippableFact(Timeout = 20000)]
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("Windows")]
        public async Task Menu_bar_visibility_and_auto_hide()
        {
            var win = this.fx.MainWindow;
            win.SetAutoHideMenuBar(true);
            await Task.Delay(500);
            (await win.IsMenuBarAutoHideAsync()).Should().BeTrue();
            win.SetMenuBarVisibility(true);
            await Task.Delay(500);
            (await win.IsMenuBarVisibleAsync()).Should().BeTrue();
        }

        [Fact(Timeout = 20000)]
        public async Task Parent_child_relationship_roundtrip()
        {
            var child = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Show = false, Width = 300, Height = 200 }, "about:blank");
            this.fx.MainWindow.SetParentWindow(null); // ensure top-level
            child.SetParentWindow(this.fx.MainWindow);
            await Task.Delay(500);
            var parent = await child.GetParentWindowAsync();
            parent.Id.Should().Be(this.fx.MainWindow.Id);
            var kids = await this.fx.MainWindow.GetChildWindowsAsync();
            kids.Select(k => k.Id).Should().Contain(child.Id);
            child.Destroy();
        }

        [SkippableFact(Timeout = 20000)]
        [SupportedOSPlatform("macOS")]
        public async Task Represented_filename_and_edited_flags()
        {
            var win = this.fx.MainWindow;
            var temp = Path.Combine(Path.GetTempPath(), "electronnet_test.txt");
            File.WriteAllText(temp, "test");
            win.SetRepresentedFilename(temp);

            await Task.Delay(500);

            var represented = await win.GetRepresentedFilenameAsync();
            represented.Should().Be(temp);

            win.SetDocumentEdited(true);

            await Task.Delay(500);

            var edited = await win.IsDocumentEditedAsync();
            edited.Should().BeTrue();

            win.SetDocumentEdited(false);
        }
    }
}
