using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.Samples.BlazorServer.Desktop;

/// <summary>
/// Everything that happens on the Electron side when the app starts: main window and application menu.
/// </summary>
public static class ElectronStartup
{
    public static async Task CreateMainWindowAsync(IServiceProvider services)
    {
        var events = services.GetRequiredService<AppEvents>();
        var windowService = services.GetRequiredService<WindowService>();

        var options = new BrowserWindowOptions
        {
            Title = "Blazor Server Sample",
            Width = 1200,
            Height = 800,
            MinWidth = 640,
            MinHeight = 480,
            Show = false,

            // Required for Blazor: injects a preload script that hides Node.js' `module` and `process`
            // globals, which would otherwise confuse Blazor's JavaScript.
            IsRunningBlazor = true,
        };

        var window = await Electron.WindowManager.CreateWindowAsync(options);

        window.OnReadyToShow += () => window.Show();

        // Hand the window to the service that the Blazor components use for "Electron control".
        windowService.Attach(window);

        Electron.Menu.SetApplicationMenu(CreateMenu(events));
    }

    private static MenuItem[] CreateMenu(AppEvents events)
    {
        // Menu clicks arrive in .NET outside of any Blazor circuit,
        // so we route them through AppEvents (see MainLayout.razor).
        MenuItem Page(string label, string uri, string accelerator) => new()
        {
            Label = label,
            Accelerator = accelerator,
            Click = () => events.RequestNavigation(uri),
        };

        var menu = new List<MenuItem>();

        if (OperatingSystem.IsMacOS())
        {
            menu.Add(new MenuItem { Role = MenuRole.appMenu });
        }

        menu.Add(new MenuItem
        {
            Label = "File",
            Submenu =
            [
                Page("Open Text File...", "/native", "CmdOrCtrl+O"),
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.quit },
            ],
        });

        // Without the edit menu, shortcuts such as Ctrl+C / Ctrl+V do not work in text inputs on macOS.
        menu.Add(new MenuItem { Role = MenuRole.editMenu });

        menu.Add(new MenuItem
        {
            Label = "View",
            Submenu =
            [
                Page("Home", "/", "CmdOrCtrl+1"),
                Page("Interactivity", "/interactivity", "CmdOrCtrl+2"),
                Page("Window Control", "/window", "CmdOrCtrl+3"),
                Page("Native Capabilities", "/native", "CmdOrCtrl+4"),
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.reload },
                new MenuItem { Role = MenuRole.toggleDevTools },
                new MenuItem { Role = MenuRole.togglefullscreen },
            ],
        });

        menu.Add(new MenuItem { Role = MenuRole.windowMenu });

        menu.Add(new MenuItem
        {
            Label = "Help",
            Submenu =
            [
                new MenuItem
                {
                    Label = "Electron.NET on GitHub",
                    Click = () => Electron.Shell.OpenExternalAsync("https://github.com/ElectronNET/Electron.NET"),
                },
            ],
        });

        return [.. menu];
    }
}
