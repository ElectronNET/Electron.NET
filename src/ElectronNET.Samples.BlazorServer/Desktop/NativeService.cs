using ElectronNET.API;
using ElectronNET.API.Entities;

namespace ElectronNET.Samples.BlazorServer.Desktop;

/// <summary>
/// Native capabilities: OS features that a web page alone cannot use.
/// </summary>
/// <remarks>
/// Note how the work is split: Electron shows the native dialog and only returns a path, while the
/// file itself is read by .NET. With Blazor Server the UI code already runs in the .NET process,
/// so there is no need to send file contents through Electron.
/// </remarks>
public sealed class NativeService
{
    public bool IsAvailable => HybridSupport.IsElectronActive;

    /// <summary>
    /// Opens a native "Open file" dialog and reads the chosen text file. Returns null when canceled.
    /// </summary>
    public async Task<(string Path, string Content)?> PickTextFileAsync()
    {
        var window = Electron.WindowManager.BrowserWindows.FirstOrDefault();

        if (window is null)
        {
            return null;
        }

        var files = await Electron.Dialog.ShowOpenDialogAsync(window, new OpenDialogOptions
        {
            Title = "Open a text file",
            Properties = [OpenDialogProperty.openFile],
            Filters =
            [
                new FileFilter { Name = "Text files", Extensions = ["txt", "md", "json", "log", "csv"] },
                new FileFilter { Name = "All files", Extensions = ["*"] },
            ],
        });

        if (files is not { Length: > 0 })
        {
            return null;
        }

        var path = files[0];
        var content = await File.ReadAllTextAsync(path);
        return (path, content);
    }

    public async Task<bool> ConfirmAsync(string message, string detail)
    {
        var window = Electron.WindowManager.BrowserWindows.FirstOrDefault();

        if (window is null)
        {
            return false;
        }

        var result = await Electron.Dialog.ShowMessageBoxAsync(window, new MessageBoxOptions(message)
        {
            Type = MessageBoxType.question,
            Title = "Please confirm",
            Detail = detail,
            Buttons = ["Yes", "No"],
        });

        return result.Response == 0;
    }

    public void ShowNotification(string title, string body, Action? onClick = null)
    {
        Electron.Notification.Show(new NotificationOptions(title, body)
        {
            OnClick = onClick,
        });
    }

    public void CopyToClipboard(string text) => Electron.Clipboard.WriteText(text);

    public Task<string> ReadClipboardAsync() => Electron.Clipboard.ReadTextAsync();

    public Task OpenExternalAsync(string url) => Electron.Shell.OpenExternalAsync(url);

    public Task ShowInFolderAsync(string path) => Electron.Shell.ShowItemInFolderAsync(path);

    public Task<string> GetElectronVersionAsync() => Electron.App.GetVersionAsync();
}
