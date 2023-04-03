using System.IO;

namespace ElectronNET.CLI.Commands.Actions
{
    public static class DeployEmbeddedElectronFiles
    {
        public static void Do(string tempPath)
        {
            var hostDistFolder = Path.Combine(tempPath, "dist");
            var vscodeFolder = Path.Combine(tempPath, ".vscode");
            var splashscreenFolder = Path.Combine(tempPath, "splashscreen");
            
            if (!Directory.Exists(hostDistFolder))
            {
                Directory.CreateDirectory(hostDistFolder);
            }

            if (!Directory.Exists(vscodeFolder))
            {
                Directory.CreateDirectory(vscodeFolder);
            }

            if (!Directory.Exists(splashscreenFolder))
            {
                Directory.CreateDirectory(splashscreenFolder);
            }

            EmbeddedFileHelper.DeployEmbeddedFile(tempPath, "package.json");
            EmbeddedFileHelper.DeployEmbeddedFile(hostDistFolder, "main.js", "dist.");
            EmbeddedFileHelper.DeployEmbeddedFile(hostDistFolder, "build-helper.js", "dist.");
            EmbeddedFileHelper.DeployEmbeddedFile(vscodeFolder, "launch.json", ".vscode.");
            EmbeddedFileHelper.DeployEmbeddedFile(vscodeFolder, "tasks.json", ".vscode.");
            EmbeddedFileHelper.DeployEmbeddedFile(splashscreenFolder, "index.html", "splashscreen.");
        }
    }
}
