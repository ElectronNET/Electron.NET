namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;

    [Collection("ElectronCollection")]
    public class IpcMainTests
    {
        private readonly ElectronFixture fx;

        public IpcMainTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact(Timeout = 20000)]
        public async Task Ipc_On_receives_message_from_renderer()
        {
            var tcs = new TaskCompletionSource<string>();
            await Electron.IpcMain.On("ipc-on-test", obj => tcs.TrySetResult(obj?.ToString() ?? string.Empty));
            await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("require('electron').ipcRenderer.send('ipc-on-test','payload123')");
            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
            result.Should().Be("payload123");
        }

        [Fact(Timeout = 20000)]
        public async Task Ipc_Once_only_fires_once()
        {
            var count = 0;
            Electron.IpcMain.Once("ipc-once-test", _ => count++);
            await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("const {ipcRenderer}=require('electron'); ipcRenderer.send('ipc-once-test','a'); ipcRenderer.send('ipc-once-test','b');");
            await Task.Delay(500);
            count.Should().Be(1);
        }

        [Fact(Timeout = 20000)]
        public async Task Ipc_RemoveAllListeners_stops_receiving()
        {
            var fired = false;
            await Electron.IpcMain.On("ipc-remove-test", _ => fired = true);
            Electron.IpcMain.RemoveAllListeners("ipc-remove-test");
            await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("require('electron').ipcRenderer.send('ipc-remove-test','x')");
            await Task.Delay(400);
            fired.Should().BeFalse();
        }

        [Fact(Timeout = 20000)]
        public async Task Ipc_OnSync_returns_value()
        {
            Electron.IpcMain.OnSync("ipc-sync-test", (obj) =>
            {
                obj.Should().NotBeNull();
                return "pong";
            });
            var ret = await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("require('electron').ipcRenderer.sendSync('ipc-sync-test','ping')");
            ret.Should().Be("pong");
        }

        [Fact(Timeout = 20000)]
        public async Task Ipc_Send_from_main_reaches_renderer()
        {
            // Listener: store raw arg; if Electron packs differently we will normalize later
            await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>(@"(function(){ const {ipcRenderer}=require('electron'); ipcRenderer.once('main-to-render',(e,arg)=>{ globalThis.__mainToRender = arg;}); return 'ready'; })();");
            Electron.IpcMain.Send(this.fx.MainWindow, "main-to-render", "hello-msg");
            string value = "";
            for (int i = 0; i < 20; i++)
            {
                var jsVal = await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("globalThis.__mainToRender === undefined ? '' : (typeof globalThis.__mainToRender === 'string' ? globalThis.__mainToRender : JSON.stringify(globalThis.__mainToRender))");
                value = jsVal?.ToString() ?? "";
                if (!string.IsNullOrEmpty(value))
                {
                    break;
                }

                await Task.Delay(100);
            }

            // Normalize possible JSON array ["hello-msg"] case
            if (value.StartsWith("[\"") && value.EndsWith("\"]"))
            {
                // Extract first element between [" and "]
                value = value.Substring(2, value.Length - 4);
            }

            value.Should().Be("hello-msg");
        }
    }
}