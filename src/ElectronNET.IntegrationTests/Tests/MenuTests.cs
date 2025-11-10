namespace ElectronNET.IntegrationTests.Tests
{
    using ElectronNET.API;
    using ElectronNET.API.Entities;

    [Collection("ElectronCollection")]
    public class MenuTests
    {
        private readonly ElectronFixture fx;
        public MenuTests(ElectronFixture fx)
        {
            this.fx = fx;
        }

        [Fact]
        public async Task ApplicationMenu_click_invokes_handler()
        {
            var clicked = false;
            var items = new[]
            {
                new MenuItem
                {
                    Label = "File",
                    Submenu = new[]
                    {
                        new MenuItem { Label = "Ping", Click = () => clicked = true },
                    },
                },
            };
            Electron.Menu.SetApplicationMenu(items);
            var targetId = items[0].Submenu[0].Id;
            await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>($"require('electron').ipcRenderer.send('integration-click-application-menu','{targetId}')");
            for (int i = 0; i < 20 && !clicked; i++)
            {
                await Task.Delay(100);
            }

            clicked.Should().BeTrue();
        }

        [Fact]
        public async Task ContextMenu_popup_registers_items()
        {
            var win = this.fx.MainWindow;
            var ctxClicked = false;
            var ctxItems = new[] { new MenuItem { Label = "Ctx", Click = () => ctxClicked = true } };
            Electron.Menu.SetContextMenu(win, ctxItems);
            var ctxId = ctxItems[0].Id;
            // simulate popup then click
            Electron.Menu.ContextMenuPopup(win);
            await this.fx.MainWindow.WebContents.ExecuteJavaScriptAsync<string>($"require('electron').ipcRenderer.send('integration-click-context-menu',{win.Id},'{ctxId}')");
            for (int i = 0; i < 20 && !ctxClicked; i++)
            {
                await Task.Delay(100);
            }

            ctxClicked.Should().BeTrue();
        }
    }
}