using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElectronNET.CLI.Commands.Actions
{
    public static class DeployEmbeddedElectronFiles
    {
        public static void Do(string tempPath)
        {
            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "main.js");
            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package.json");
            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package-lock.json");

            string hostApiFolder = Path.Combine(tempPath, "api");
            if (Directory.Exists(hostApiFolder) == false)
            {
                Directory.CreateDirectory(hostApiFolder);
            }
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
        }
    }
}
