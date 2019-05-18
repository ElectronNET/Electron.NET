using ElectronNET.CLI.Commands.Actions;
using ElectronNET.CLI.Config;
using ElectronNET.CLI.Config.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands {

    /// <summary> Build electron Command. </summary>
    public class BuildCommand : ICommand {

        /// <summary> General Application Settings. </summary>
        /// <value> General Application Settings. </value>
        private AppSettings Appcfg { get; set; }

        /// <summary> Command specific settings. </summary>
        /// <value> Command specific settings. </value>
        private BuildConfig Cmdcfg { get; set; }


        /// <summary> Build electron Command Execute. </summary>
        /// <returns> Build electron Command Task. </returns>
        public Task<bool> ExecuteAsync() {
            return Task.Run(() => {
                Console.WriteLine("Build Electron Application...");

                // Read in the configuration
                Appcfg = SettingsLoader.Settings;
                Cmdcfg = (BuildConfig) Appcfg.CommandConfig;

                // Publish the .net project to the run path
                if (!DotnetPublish())
                    return false;

                // Setup the node_modules directory using npm or the selected package manager
                if (!SetupNodeModules())
                    return false;

                // Build the ElectronHostHook files
                if (!BuildElectronHostHook())
                    return false;

                // Build the application
                if (!BuildApp())
                    return false;

                Console.WriteLine("... done");

                return true;
            });
        }

        /// <summary> Do a dotnet publish. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool DotnetPublish() {
            Console.WriteLine($"Build ASP.NET Core App for {Cmdcfg.RuntimeIdentifier}...");
            Console.WriteLine($"Executing dotnet publish in this directory: {Cmdcfg.TmpBuildPath}");
            var tempBinPath = Path.Combine(Cmdcfg.TmpBuildPath, "bin");
            Console.WriteLine(
                $"Build ASP.NET Core App for {Cmdcfg.RuntimeIdentifier} under {Cmdcfg.DotnetConfiguration}-Configuration...");
            var resultCode = ProcessHelper.CmdExecute(
                $"dotnet publish -r {Cmdcfg.RuntimeIdentifier} -c {Cmdcfg.DotnetConfiguration} --output \"{tempBinPath}\"",
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

            DeployEmbeddedElectronFiles.Do(Cmdcfg.TmpBuildPath);
            if (Cmdcfg.PackageJsonFile != null) {
                Console.WriteLine("Copying custom package.json.");
                File.Copy(Cmdcfg.PackageJsonFile, Path.Combine(Cmdcfg.TmpBuildPath, "package.json"), true);
            }

            var checkForNodeModulesDirPath = Path.Combine(Cmdcfg.TmpBuildPath, "node_modules");
            if (Directory.Exists(checkForNodeModulesDirPath) == false || Cmdcfg.ForceNpmInstall ||
                Cmdcfg.PackageJsonFile != null) {
                Console.WriteLine($"Start {Cmdcfg.NpmCommand.ToInstallCmd()}...");
                ProcessHelper.CmdExecute($"{Cmdcfg.NpmCommand.ToInstallCmd()} --production", Cmdcfg.TmpBuildPath);
            }

            // Install typescript globally
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Console.WriteLine($"Start {Cmdcfg.NpmCommand.ToInstallCmd()} typescript...");
                ProcessHelper.CmdExecute($"{Cmdcfg.NpmCommand.ToInstallCmd()} typescript --global", Cmdcfg.TmpBuildPath);
            }
            else {
                // TODO this might not be needed for Linux / Osx needs testing
                Console.WriteLine(
                    $"Electron Builder - make sure you invoke 'sudo {Cmdcfg.NpmCommand.ToInstallCmd()} typescript --global' at " +
                    Cmdcfg.TmpBuildPath + $" manually. Sry.");
            }

            // TODO check version of typescript with tsc --version
            // This is due to Microsoft addin they're own version of tsc to the path by default
            // https://stackoverflow.com/questions/38191287/error-ts5023-unknown-compiler-option-p

            Console.WriteLine($"Start {Cmdcfg.NpmCommand.ToInstallCmd()} electron-builder...");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // Works proper on Windows... 
                ProcessHelper.CmdExecute($"{Cmdcfg.NpmCommand.ToInstallCmd()} electron-builder --global", Cmdcfg.TmpBuildPath);
            }
            else {
                // ToDo: find another solution or document it proper
                // GH Issue https://github.com/electron-userland/electron-prebuilt/issues/48
                Console.WriteLine(
                    $"Electron Builder - make sure you invoke 'sudo {Cmdcfg.NpmCommand.ToInstallCmd()} electron-builder --global' at " +
                    Cmdcfg.TmpBuildPath + $" manually. Sry.");
            }
            return true;
        }



        /// <summary> Builds the ElectronHostHook files. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool BuildElectronHostHook() {
            Console.WriteLine("ElectronHostHook handling started...");
            if (Directory.Exists(Cmdcfg.ElectronHostHookPath)) {
                var hosthookDir = Path.Combine(Cmdcfg.TmpBuildPath, "ElectronHostHook");
                DirectoryCopy.Do(Cmdcfg.ElectronHostHookPath, hosthookDir, true, new List<string>() {"node_modules"});

                Console.WriteLine($"Start {Cmdcfg.NpmCommand.ToInstallCmd()} for hosthooks...");
                ProcessHelper.CmdExecute($"{Cmdcfg.NpmCommand.ToInstallCmd()} --production", hosthookDir);

                // ToDo: Global TypeScript installation is needed for ElectronHostHook
                //string tscPath = Path.Combine(tempPath, "node_modules", ".bin");

                // ToDo: Not sure if this runs under linux/macos
                ProcessHelper.CmdExecute($@"tsc -p . --sourceMap false", hosthookDir);
            }

            return true;
        }

        /// <summary> Builds the application. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool BuildApp() {
            Console.WriteLine("Build Electron Desktop Application...");
            Console.WriteLine($"Executing electron magic in this directory: {Cmdcfg.TmpBuildPath}");
            
            // ToDo: Need a solution for --asar support

            // ToDo: Make the same thing easer with native c# - we can save a tmp file in production code :)
            Console.WriteLine("Create electron-builder configuration file...");
            ProcessHelper.CmdExecute($"node build-helper.js", Cmdcfg.TmpBuildPath);

            Console.WriteLine($"Package Electron App for Platform {Cmdcfg.ElectronPackerPlatform}...");
            ProcessHelper.CmdExecute(
                $"electron-builder . --config=./bin/electron-builder.json --platform={Cmdcfg.ElectronPackerPlatform} --arch={Cmdcfg.ElectronArch} {Cmdcfg.ElectronParams}",
                Cmdcfg.TmpBuildPath);

            return true;
        }
    }
}
