using System.IO;

namespace ElectronNET.CLI.Commands.Actions
{
    public static class DeployEmbeddedElectronFiles
    {
        public static void Do(string tempPath)
        {
            EmbeddedFileHelper.PrintAllResources();

            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "main.js");
            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package.json");
            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "build-helper.js");

            string hostApiFolder = Path.Combine(tempPath, "api");

            //Create the HostApi folder, if it doesnt already exist
            Directory.CreateDirectory(hostApiFolder);

            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "ipc.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "app.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "browserWindows.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "commandLine.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "dialog.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "dock.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "menu.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "notification.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "tray.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "webContents.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "globalShortcut.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "shell.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "screen.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "clipboard.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "autoUpdater.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "browserView.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "powerMonitor.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "nativeTheme.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "process.js", "api.");

            string splashscreenFolder = Path.Combine(tempPath, "splashscreen");

            //Create SplashScreen folder, if it doesnt already exist
            Directory.CreateDirectory(splashscreenFolder);

            EmbeddedFileHelper.DeployEmbeddedFile(splashscreenFolder, "index.html", "splashscreen.");
        }
    }
}
