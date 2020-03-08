using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ElectronNET.CLI.Commands.Actions;

namespace ElectronNET.CLI.Commands
{
    public class DebugElectronCommand : ICommand
    {
        public const string COMMAND_NAME = "debug";
        public const string COMMAND_DESCRIPTION = "Starting With auto build";
        public const string COMMAND_ARGUMENTS = "<Path> from ASP.NET Core Project.";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        private string[] _args;

        public DebugElectronCommand(string[] args)
        {
            _args = args;
        }

        private string _aspCoreProjectPath = "project-path";
        private string _arguments = "args";
        private string _manifest = "manifest";

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Start Electron Desktop Application...");

                SimpleCommandLineParser parser = new SimpleCommandLineParser();
                parser.Parse(_args);

                string aspCoreProjectPath = "";

                if (parser.Arguments.ContainsKey(_aspCoreProjectPath))
                {
                    string projectPath = parser.Arguments[_aspCoreProjectPath].First();
                    if (Directory.Exists(projectPath))
                    {
                        aspCoreProjectPath = projectPath;
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

                var platformInfo = GetTargetPlatformInformation.Do(string.Empty, string.Empty);

                string tempBinPath = Path.Combine(tempPath, "bin");
                if (!Directory.Exists(tempBinPath))
                {
                    Directory.CreateDirectory(tempBinPath);
                }

                // mklink wwwroot folder
                var resultCode = 0;

                // if not exists then create mklink
                if (!Directory.Exists($"{tempBinPath}\\wwwroot"))
                {
                    resultCode = ProcessHelper.CmdExecute($"mklink /D {tempBinPath}\\wwwroot {aspCoreProjectPath}\\wwwroot", aspCoreProjectPath);
                }

                // now mklink all the files in bin and see if we can find any of these dotnetcore version
                var debugFiles2 = $"{aspCoreProjectPath}\\bin\\Debug\\netcoreapp2.0";
                var debugFiles21 = $"{aspCoreProjectPath}\\bin\\Debug\\netcoreapp2.1";
                var debugFiles3 = $"{aspCoreProjectPath}\\bin\\Debug\\netcoreapp3.0";
                var debugFiles31 = $"{aspCoreProjectPath}\\bin\\Debug\\netcoreapp3.1";
                var finalPath = "";

                if (Directory.Exists(debugFiles2)) finalPath = debugFiles2;
                if (Directory.Exists(debugFiles21)) finalPath = debugFiles21;
                if (Directory.Exists(debugFiles3)) finalPath = debugFiles3;
                if (Directory.Exists(debugFiles31)) finalPath = debugFiles31;

                foreach (var item in Directory.GetFiles(finalPath, "*.*", SearchOption.TopDirectoryOnly))
                {
                    // if not exists then create mklink
                    if (!File.Exists($"{tempBinPath}\\{System.IO.Path.GetFileName(item)}"))
                    {
                        resultCode = ProcessHelper.CmdExecute($"mklink {tempBinPath}\\{System.IO.Path.GetFileName(item)} {item}", aspCoreProjectPath);
                    }
                }

                if (resultCode != 0)
                {
                    Console.WriteLine("Error occurred during dotnet publish: " + resultCode);
                    return false;
                }

                DeployEmbeddedElectronFiles.Do(tempPath);

                var nodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                Console.WriteLine("node_modules missing in: " + nodeModulesDirPath);

                Console.WriteLine("Start npm install...");
                ProcessHelper.CmdExecute("npm install", tempPath);

                Console.WriteLine("ElectronHostHook handling started...");

                string electronhosthookDir = Path.Combine(Directory.GetCurrentDirectory(), "ElectronHostHook");

                if (Directory.Exists(electronhosthookDir))
                {
                    string hosthookDir = Path.Combine(tempPath, "ElectronHostHook");
                    DirectoryCopy.Do(electronhosthookDir, hosthookDir, true, new List<string>() { "node_modules" });

                    Console.WriteLine("Start npm install for typescript & hosthooks...");
                    ProcessHelper.CmdExecute("npm install", hosthookDir);

                    // ToDo: Not sure if this runs under linux/macos
                    ProcessHelper.CmdExecute(@"npx tsc -p ../../ElectronHostHook", tempPath);
                }

                string arguments = "";

                if (parser.Arguments.ContainsKey(_arguments))
                {
                    arguments = string.Join(' ', parser.Arguments[_arguments]);
                }

                if (parser.Arguments.ContainsKey(_manifest))
                {
                    arguments += " --manifest=" + parser.Arguments[_manifest].First();
                }

                string path = Path.Combine(tempPath, "node_modules", ".bin");
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                if (isWindows)
                {
                    Console.WriteLine("Invoke electron.cmd - in dir: " + path);
                    ProcessHelper.CmdExecute(@"electron.cmd ""..\..\main.js"" " + arguments, path, (data) =>
                    {
                        if (Regex.Match(data, @"^electronPort=[0-9]+,electronWebPort=[0-9]+$", RegexOptions.Multiline).Success)
                        {
                            var args = "/" + data.Replace(",", " /");
                            ProcessHelper.CmdExecute($"dotnet watch run {args}", aspCoreProjectPath);
                        }

                    });
                }
                else
                {
                    Console.WriteLine("Invoke electron - in dir: " + path);
                    ProcessHelper.CmdExecute(@"./electron ""../../main.js"" " + arguments, path, (data) =>
                    {
                        if (Regex.Match(data, @"^electronPort=[0-9]+,electronWebPort=[0-9]+$", RegexOptions.Multiline).Success)
                        {
                            var args = "/" + data.Replace(",", " /");
                            ProcessHelper.CmdExecute($"dotnet watch run {args}", aspCoreProjectPath);
                        }

                    });
                }

                return true;
            });
        }


    }
}
