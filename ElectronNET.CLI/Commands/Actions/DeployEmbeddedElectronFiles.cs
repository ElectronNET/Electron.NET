using System.IO;

namespace ElectronNET.CLI.Commands.Actions {

    /// <summary> Deploy embedded files. </summary>
    public static class DeployEmbeddedElectronFiles {

        /// <summary> Deploys files embedded in the cli application </summary>
        /// <param name="tempPath"> Deployment path. </param>
        public static void Do(string tempPath) {
            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "main.js");
            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package.json");
            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "build-helper.js");

            var hostApiFolder = Path.Combine(tempPath, "api");
            if (Directory.Exists(hostApiFolder) == false)
                Directory.CreateDirectory(hostApiFolder);

            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "ipc.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "app.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "browserWindows.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "dialog.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "menu.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "notification.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "tray.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "webContents.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "globalShortcut.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "shell.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "screen.js", "api.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostApiFolder, "clipboard.js", "api.");

            var splashscreenFolder = Path.Combine(tempPath, "splashscreen");
            if (Directory.Exists(splashscreenFolder) == false)
                Directory.CreateDirectory(splashscreenFolder);
            EmbeddedFileHelper.DeployEmbeddedFile(splashscreenFolder, "index.html", "splashscreen.");
        }
    }
}
