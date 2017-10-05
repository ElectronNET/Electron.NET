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
                Console.WriteLine("Start Electron...");

                string aspCoreProjectPath = "";
                string electronHostProjectPath = "";

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

                using (Process cmd = new Process())
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;

                    cmd.Start();

                    cmd.StandardInput.WriteLine("cd " + aspCoreProjectPath);
                    cmd.StandardInput.Flush();

                    cmd.StandardInput.WriteLine("dotnet restore");
                    cmd.StandardInput.Flush();

                    cmd.StandardInput.WriteLine("dotnet publish -r win10-x64 --output bin/dist/win");
                    cmd.StandardInput.Flush();

                    cmd.StandardInput.WriteLine(@"..\ElectronNET.Host\node_modules\.bin\electron.cmd ""..\ElectronNET.Host\main.js""");
                    cmd.StandardInput.Flush();

                    cmd.StandardInput.Close();
                }

                return true;
            });
        }
    }
}
