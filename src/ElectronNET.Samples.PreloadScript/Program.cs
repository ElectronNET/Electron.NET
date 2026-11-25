using System.Runtime.InteropServices;
using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddElectron();

builder.WebHost.UseElectron(args, async () =>
{
    // 1. Register the IPC endpoints *before* the window loads, so the page can use them right away.
    await RegisterIpcHandlersAsync();

    // 2. Create the window and inject the preload script.
    //    Electron loads the preload script from disk, so we need an absolute path. The script is
    //    copied to the output directory by the .csproj, i.e., it sits next to the .NET assemblies.
    var preloadPath = Path.Combine(AppContext.BaseDirectory, "Preload", "preload.js");

    var options = new BrowserWindowOptions
    {
        Width = 900,
        Height = 700,
        Show = false,
        WebPreferences = new WebPreferences
        {
            Preload = preloadPath,

            // Secure defaults: the web content has no access to Node.js or Electron.
            // Everything it may do is exposed explicitly by the preload script via contextBridge.
            NodeIntegration = false,
            ContextIsolation = true,
            Sandbox = true,
        },
    };

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
    {
        options.AutoHideMenuBar = true;
    }

    var window = await Electron.WindowManager.CreateWindowAsync(options);

    window.OnReadyToShow += () => window.Show();

    // 3. Push messages from .NET to the web content.
    StartClock(window);
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();

static async Task RegisterIpcHandlersAsync()
{
    // Fire-and-forget message (ipcRenderer.send) which we answer with a message of our own (webContents.send).
    await Electron.IpcMain.On("greet", payload =>
    {
        var name = payload as string;
        var greeting = string.IsNullOrWhiteSpace(name)
            ? "Hello, anonymous!"
            : $"Hello, {name}! This greeting was created in .NET.";

        var window = Electron.WindowManager.BrowserWindows.FirstOrDefault();

        if (window is not null)
        {
            Electron.IpcMain.Send(window, "greeting", greeting);
        }
    });

    // Request / response (ipcRenderer.invoke). The return value resolves the promise in the renderer.
    Electron.IpcMain.Handle("get-runtime-info", _ => new
    {
        Framework = RuntimeInformation.FrameworkDescription,
        Os = RuntimeInformation.OSDescription,
        Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
        ProcessId = Environment.ProcessId,
        MachineName = Environment.MachineName,
    });

    // Request / response with an argument. Objects arrive as Dictionary<string, object>.
    Electron.IpcMain.Handle("add", payload =>
    {
        if (payload is Dictionary<string, object> values &&
            values.TryGetValue("a", out var a) &&
            values.TryGetValue("b", out var b))
        {
            return Convert.ToDouble(a) + Convert.ToDouble(b);
        }

        return double.NaN;
    });
}

static void StartClock(BrowserWindow window)
{
    var timer = new Timer(_ => Electron.IpcMain.Send(window, "clock", DateTime.Now.ToString("T")), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    window.OnClosed += () => timer.Dispose();
}
