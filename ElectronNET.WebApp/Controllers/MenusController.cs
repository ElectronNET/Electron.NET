using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API.Entities;
using ElectronNET.API;

namespace ElectronNET.WebApp.Controllers
{
    public class MenusController : Controller
    {
        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                var menu = new MenuItem[] {
                new MenuItem { Label = "Edit", Submenu = new MenuItem[] {
                    new MenuItem { Label = "Undo", Accelerator = "CmdOrCtrl+Z", Role = MenuRole.undo },
                    new MenuItem { Label = "Redo", Accelerator = "Shift+CmdOrCtrl+Z", Role = MenuRole.redo },
                    new MenuItem { Type = MenuType.separator },
                    new MenuItem { Label = "Cut", Accelerator = "CmdOrCtrl+X", Role = MenuRole.cut },
                    new MenuItem { Label = "Copy", Accelerator = "CmdOrCtrl+C", Role = MenuRole.copy },
                    new MenuItem { Label = "Paste", Accelerator = "CmdOrCtrl+V", Role = MenuRole.paste },
                    new MenuItem { Label = "Select All", Accelerator = "CmdOrCtrl+A", Role = MenuRole.selectall }
                }
                },
                new MenuItem { Label = "View", Submenu = new MenuItem[] {
                    new MenuItem
                    {
                        Label = "Reload",
                        Accelerator = "CmdOrCtrl+R",
                        Click = () =>
                        {
                            // on reload, start fresh and close any old
                            // open secondary windows
                            Electron.WindowManager.BrowserWindows.ToList().ForEach(browserWindow => {
                                if(browserWindow.Id != 1)
                                {
                                    browserWindow.Close();
                                }
                                else
                                {
                                    browserWindow.Reload();
                                }
                            });
                        }
                    },
                    new MenuItem
                    {
                        Label = "Toggle Full Screen",
                        Accelerator = "CmdOrCtrl+F",
                        Click = async () =>
                        {
                            bool isFullScreen = await Electron.WindowManager.BrowserWindows.First().IsFullScreenAsync();
                            Electron.WindowManager.BrowserWindows.First().SetFullScreen(!isFullScreen);
                        }
                    },
                    new MenuItem
                    {
                        Label = "Open Developer Tools",
                        Accelerator = "CmdOrCtrl+I",
                        Click = () => Electron.WindowManager.BrowserWindows.First().WebContents.OpenDevTools()
                    },
                    new MenuItem
                    {
                        Type = MenuType.separator
                    },
                    new MenuItem
                    {
                        Label = "App Menu Demo",
                        Click = async () => {
                            var options = new MessageBoxOptions("This demo is for the Menu section, showing how to create a clickable menu item in the application menu.");
                            options.Type = MessageBoxType.info;
                            options.Title = "Application Menu Demo";
                            await Electron.Dialog.ShowMessageBoxAsync(options);
                        }
                    }
                }
                },
                new MenuItem { Label = "Window", Role = MenuRole.window, Submenu = new MenuItem[] {
                     new MenuItem { Label = "Minimize", Accelerator = "CmdOrCtrl+M", Role = MenuRole.minimize },
                     new MenuItem { Label = "Close", Accelerator = "CmdOrCtrl+W", Role = MenuRole.close }
                }
                },
                new MenuItem { Label = "Help", Role = MenuRole.help, Submenu = new MenuItem[] {
                    new MenuItem
                    {
                        Label = "Learn More",
                        Click = async () => await Electron.Shell.OpenExternalAsync("https://github.com/ElectronNET")
                    }
                }
                }
            };

                Electron.Menu.SetApplicationMenu(menu);

                CreateContextMenu();
            }

            return View();
        }

        private void CreateContextMenu()
        {
            var menu = new MenuItem[]
            {
                new MenuItem
                {
                    Label = "Hello",
                    Click = async () => await Electron.Dialog.ShowMessageBoxAsync("Electron.NET rocks!")
                },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Label = "Electron.NET", Type = MenuType.checkbox, Checked = true }
            };

            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            Electron.Menu.SetContextMenu(mainWindow, menu);

            Electron.IpcMain.On("show-context-menu", (args) =>
            {
                Electron.Menu.ContextMenuPopup(mainWindow);
            });
        }
    }
}