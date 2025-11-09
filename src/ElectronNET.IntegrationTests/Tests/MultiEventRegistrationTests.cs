namespace ElectronNET.IntegrationTests.Tests
{
    [Collection("ElectronCollection")]
    public class MultiEventRegistrationTests
    {
        private readonly ElectronFixture fx;

        public MultiEventRegistrationTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        private static async Task<bool> WaitAllOrTimeout(TimeSpan timeout, params Task[] tasks)
        {
            var all = Task.WhenAll(tasks);
            var completed = await Task.WhenAny(all, Task.Delay(timeout));
            return ReferenceEquals(completed, all) && all.IsCompletedSuccessfully;
        }

        [Fact]
        public async Task BrowserWindow_OnResize_multiple_handlers_called()
        {
            var win = this.fx.MainWindow;
            var h1 = new TaskCompletionSource();
            var h2 = new TaskCompletionSource();
            var h3 = new TaskCompletionSource();

            win.OnResize += () => h1.TrySetResult();
            win.OnResize += () => h2.TrySetResult();
            win.OnResize += () => h3.TrySetResult();

            var size = await win.GetSizeAsync();
            // trigger resize
            win.SetSize(size[0] + 20, size[1] + 10);

            var ok = await WaitAllOrTimeout(TimeSpan.FromSeconds(5), h1.Task, h2.Task, h3.Task);

            if (!ok)
            {
                throw new Xunit.Sdk.XunitException($"Not all events were fired: \nEvent1 fired: {h1.Task.IsCompleted}\nEvent2 fired: {h2.Task.IsCompleted}\nEvent3 fired: {h3.Task.IsCompleted}");
            }
        }

        [Fact]
        public async Task WebContents_OnDomReady_multiple_handlers_called()
        {
            var wc = this.fx.MainWindow.WebContents;
            var r1 = new TaskCompletionSource();
            var r2 = new TaskCompletionSource();

            wc.OnDomReady += () => r1.TrySetResult();
            wc.OnDomReady += () => r2.TrySetResult();

            await wc.LoadURLAsync("about:blank");

            var ok = await WaitAllOrTimeout(TimeSpan.FromSeconds(2), r1.Task, r2.Task);

            if (!ok)
            {
                throw new Xunit.Sdk.XunitException($"Not all events were fired: \nEvent1 fired: {r1.Task.IsCompleted}\nEvent2 fired: {r2.Task.IsCompleted}");
            }
        }
    }
}