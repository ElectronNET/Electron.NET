using ElectronNET.CLI.Commands.Actions;
using ElectronNET.CLI.Config;
using ElectronNET.CLI.Config.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands {

    /// <summary> Start electron Command. </summary>
    public class StartCommand : ICommand {

        /// <summary> General Application Settings. </summary>
        /// <value> General Application Settings. </value>
        private AppSettings Appcfg { get; set; }

        /// <summary> Command specific settings. </summary>
        /// <value> Command specific settings. </value>
        private StartConfig Cmdcfg { get; set; }

        /// <summary> Start electron Command Execute. </summary>
        /// <returns> Start electron Command Task. </returns>
        public Task<bool> ExecuteAsync() {
            return Task.Run(() => {
                Console.WriteLine("Start Electron Desktop Application...");

                // Read in the configuration
                Appcfg = SettingsLoader.Settings;
                Cmdcfg = (StartConfig)Appcfg.CommandConfig;

                // Create a temp directory for running / debugging
                if (Cmdcfg.RunPath == null) {
                    Cmdcfg.RunPath = Path.Combine(Cmdcfg.ProjectPath, "bin", "Host");
                    if (Directory.Exists(Cmdcfg.RunPath) == false)
                        Directory.CreateDirectory(Cmdcfg.RunPath);

                }

                // Publish the .net project to the run path
                if (!DotnetPublish())
                    return false;

                // Copy over the host files needed for electron
                DeployEmbeddedElectronFiles.Do(Cmdcfg.RunPath);

                // Setup the node_modules directory using npm or the selected package manager
                if (!SetupNodeModules())
                    return false;

                // Setup the electron hosthook
                if (!SetupElectronHostHook())
                    return false;

                // Launch electron
                if (!LaunchElectron())
                    return false;

                return true;
            });
        }


        /// <summary> Do a dotnet publish. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool DotnetPublish() {
            var tempBinPath = Path.Combine(Cmdcfg.RunPath, "bin");
            var resultCode = ProcessHelper.CmdExecute(
                $"dotnet publish -r {Cmdcfg.RuntimeIdentifier} --output \"{tempBinPath}\" {Cmdcfg.DotnetAdditionalOpts}",
                Cmdcfg.ProjectPath);
            if (resultCode != 0) {
                Console.WriteLine($"Error occurred during dotnet publish: {resultCode}");
                return false;
            }
            return true;
        }

        /// <summary> Setup the node modules directory. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool SetupNodeModules() {
            var NodeModulesDirPath = Path.Combine(Cmdcfg.RunPath, "node_modules");

            if (!Directory.Exists(NodeModulesDirPath))
                Console.WriteLine($"node_modules missing in: {NodeModulesDirPath}");

            if (Cmdcfg.ForceNpmInstall) {
                if (Directory.Exists(NodeModulesDirPath)) {
                    Console.WriteLine("node_modules detected but deleting due to force being enabled");
                    Directory.Delete(NodeModulesDirPath,true);
                }
            }

            if (!Directory.Exists(NodeModulesDirPath) || Cmdcfg.ForceNpmInstall) {
                Console.WriteLine($"Start {Cmdcfg.NpmCommand.ToInstallCmd()} ...");
                ProcessHelper.CmdExecute(Cmdcfg.NpmCommand.ToInstallCmd(), Cmdcfg.RunPath);
                Console.WriteLine("install to node_modules complete");
            }
            else {
                Console.WriteLine("Skipping install to node_modules");
                Console.WriteLine($"directory already exists: {NodeModulesDirPath}");
            }
            return true;
        }


        /// <summary> Sets up the electron host hook. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool SetupElectronHostHook() {
            Console.WriteLine("ElectronHostHook handling started...");
            var electronhosthookDir = Path.Combine(Cmdcfg.ProjectPath, "ElectronHostHook");

            if (Directory.Exists(electronhosthookDir)) {
                Console.WriteLine("ElectronHostHook directory found.");

                var hosthookDir = Path.Combine(Cmdcfg.RunPath, "ElectronHostHook");
                DirectoryCopy.Do(electronhosthookDir, hosthookDir, true, new List<string>() { "node_modules" });

                Console.WriteLine($"Start {Cmdcfg.NpmCommand.ToInstallCmd()} for hosthooks...");
                ProcessHelper.CmdExecute(Cmdcfg.NpmCommand.ToInstallCmd(), hosthookDir);

                var tscPath = Path.Combine(Cmdcfg.RunPath, "node_modules", ".bin");
                // ToDo: Not sure if this runs under linux/macos
                ProcessHelper.CmdExecute(@"tsc -p ../../ElectronHostHook", tscPath);
            }

            return true;
        }


        /// <summary> Launches electron. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool LaunchElectron() {
            var nodebinpath = Path.Combine(Cmdcfg.RunPath, "node_modules", ".bin");
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (isWindows) {
                Console.WriteLine($"Invoke electron.cmd - in dir: {nodebinpath}");
                ProcessHelper.CmdExecute(
                    $@"electron.cmd {Cmdcfg.ElectronParams} ""..\..\main.js""",
                    nodebinpath);
            }
            else {
                Console.WriteLine($"Invoke electron - in dir: {nodebinpath}");
                ProcessHelper.CmdExecute(
                    $@"./electron {Cmdcfg.ElectronParams} ""../../main.js""",
                    nodebinpath);
            }
            return true;
        }

    }
}
