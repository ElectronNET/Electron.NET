namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.Common;
    using ElectronNET.IntegrationTests.Common;

    [Collection("ElectronCollection")]
    public class IpcMainTests : IntegrationTestBase
    {
        public IpcMainTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact]
        public async Task Ipc_On_receives_message_from_renderer()
        {
            object received = null;

            var tcs = new TaskCompletionSource<string>();
            await Electron.IpcMain.On("ipc-on-test", obj =>
            {
                received = obj;
                tcs.TrySetResult(obj as string);
            });

            await this.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("require('electron').ipcRenderer.send('ipc-on-test','payload123')");

            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

            received.Should().BeOfType<string>();
            received.Should().Be("payload123");
            result.Should().Be("payload123");
        }

        [IntegrationFact]
        public async Task Ipc_Once_only_fires_once()
        {
            var count = 0;
            Electron.IpcMain.Once("ipc-once-test", _ => count++);
            await this.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("const {ipcRenderer}=require('electron'); ipcRenderer.send('ipc-once-test','a'); ipcRenderer.send('ipc-once-test','b');");
            await Task.Delay(500.ms());
            count.Should().Be(1);
        }

        [IntegrationFact]
        public async Task Ipc_RemoveAllListeners_stops_receiving()
        {
            var fired = false;
            await Electron.IpcMain.On("ipc-remove-test", _ => fired = true);
            Electron.IpcMain.RemoveAllListeners("ipc-remove-test");
            await this.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("require('electron').ipcRenderer.send('ipc-remove-test','x')");
            await Task.Delay(400.ms());
            fired.Should().BeFalse();
        }

        [IntegrationFact]
        public async Task Ipc_OnSync_returns_value()
        {
            object received = null;

            Electron.IpcMain.OnSync("ipc-sync-test", (obj) =>
            {
                received = obj;
                return "pong";
            });
            var ret = await this.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("require('electron').ipcRenderer.sendSync('ipc-sync-test','ping')");

            received.Should().BeOfType<string>();
            received.Should().Be("ping");

            ret.Should().Be("pong");
        }

        [IntegrationFact]
        public async Task Ipc_Send_from_main_reaches_renderer()
        {
            // Listener: store raw arg; if Electron packs differently we will normalize later
            await this.MainWindow.WebContents.ExecuteJavaScriptAsync<string>(@"(function(){ const {ipcRenderer}=require('electron'); ipcRenderer.once('main-to-render',(e,arg)=>{ globalThis.__mainToRender = arg;}); return 'ready'; })();");
            Electron.IpcMain.Send(this.MainWindow, "main-to-render", "hello-msg");
            string value = "";
            for (int i = 0; i < 20; i++)
            {
                var jsVal = await this.MainWindow.WebContents.ExecuteJavaScriptAsync<string>("globalThis.__mainToRender === undefined ? '' : (typeof globalThis.__mainToRender === 'string' ? globalThis.__mainToRender : JSON.stringify(globalThis.__mainToRender))");
                value = jsVal?.ToString() ?? "";
                if (!string.IsNullOrEmpty(value))
                {
                    break;
                }

                await Task.Delay(100.ms());
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