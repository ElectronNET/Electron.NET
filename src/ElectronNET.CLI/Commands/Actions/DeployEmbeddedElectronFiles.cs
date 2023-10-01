using System;
using System.IO;

namespace ElectronNET.CLI.Commands.Actions;

public static class DeployEmbeddedElectronFiles
{
    public static void Do(string tempPath, bool overrideHook)
    {
        var includes = new string[]
        {
            "ElectronHostAPI",
            "ElectronHostHook",
            "splashscreen",
            "dist",
            "src"
        };

        EmbeddedFileHelper.DeployEmbeddedFolder(
            tempPath,
            "ElectronHost",
            includes,
            overrideHook
                ? new string[] { "ElectronHostHook" }
                : Array.Empty<string>());
    }
}
