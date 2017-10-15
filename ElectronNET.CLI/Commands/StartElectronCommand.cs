using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands
{
    public class StartElectronCommand : ICommand
    {
        public const string COMMAND_NAME = "start";
        public const string COMMAND_DESCRIPTION = "Start your ASP.NET Core Application with Electron.";
        public const string COMMAND_ARGUMENTS = "<Path> from ASP.NET Core Project.";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private string[] _args;

        public StartElectronCommand(string[] args)
        {
            _args = args;
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Start Electron Desktop Application...");

                string aspCoreProjectPath = "";

                if (_args.Length > 0)
                {
                    if (Directory.Exists(_args[0]))
                    {
                        aspCoreProjectPath = _args[0];
                    }
                }
                else
                {
                    aspCoreProjectPath = Directory.GetCurrentDirectory();
                }

                string tempPath = Path.Combine(aspCoreProjectPath, "obj", "Host");
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }

                string tempBinPath = Path.Combine(tempPath, "bin");
                ProcessHelper.CmdExecute($"dotnet publish -r win10-x64 --output \"{tempBinPath}\"", aspCoreProjectPath);

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

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

                ProcessHelper.CmdExecute(@"electron.cmd ""..\..\main.js""", Path.Combine(tempPath, "node_modules", ".bin"), false, false);

                return true;
            });
        }
    }
}
