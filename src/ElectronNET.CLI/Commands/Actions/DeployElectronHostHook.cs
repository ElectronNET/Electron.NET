using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ElectronNET.CLI.Commands.Actions
{
    public static class DeployElectronHostHook
    {
        public static void Do(string basePath, string hostHookPath)
        {
            if (!string.IsNullOrEmpty(hostHookPath))
            {
                Console.WriteLine("ElectronHostHook handling started...");

                string hostHookTargetPath = Path.Combine(basePath, "ElectronHostHook");

                DirectoryCopy.Do(hostHookPath, hostHookTargetPath, true, new List<string>() { "node_modules" });

                string hostHookPackagePath = Path.Combine(hostHookTargetPath, "package.json");

                var hostHookJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(hostHookPackagePath));
                hostHookJson["name"] = JsonSerializer.SerializeToElement("@electron-host/hook");

                File.WriteAllText(hostHookPackagePath, JsonSerializer.Serialize(hostHookJson, new JsonSerializerOptions { WriteIndented = true }));
            }
        }
    }
}
