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
        private AppSettings appcfg { get; set; }

        /// <summary> Command specific settings. </summary>
        /// <value> Command specific settings. </value>
        private BuildConfig cmdcfg { get; set; }

        /// <summary> Build electron Command Execute. </summary>
        /// <returns> Build electron Command Task. </returns>
        public Task<bool> ExecuteAsync() {
            return Task.Run(() => {
                Console.WriteLine("Build Electron Application...");

                // Read in the configuration
                appcfg = SettingsLoader.Settings;
                cmdcfg = (BuildConfig) appcfg.CommandConfig;

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
            Console.WriteLine($"Build ASP.NET Core App for {cmdcfg.RuntimeIdentifier}...");
            Console.WriteLine($"Executing dotnet publish in this directory: {cmdcfg.BuildPath}");
            var tempBinPath = Path.Combine(cmdcfg.BuildPath, "bin");
            Console.WriteLine(
                $"Build ASP.NET Core App for {cmdcfg.RuntimeIdentifier} under {cmdcfg.DotnetConfiguration}-Configuration...");
            var resultCode = ProcessHelper.CmdExecute(
                $"dotnet publish -r {cmdcfg.RuntimeIdentifier} -c {cmdcfg.DotnetConfiguration} --output \"{tempBinPath}\"",
                cmdcfg.ProjectPath);
            if (resultCode != 0) {
                Console.WriteLine($"Error occurred during dotnet publish: {resultCode}");
                return false;
            }
            return true;
        }


        /// <summary> Setup the node modules directory. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool SetupNodeModules() {

            DeployEmbeddedElectronFiles.Do(cmdcfg.BuildPath);
            var nodeModulesDirPath = Path.Combine(cmdcfg.BuildPath, "node_modules");
            if (cmdcfg.PackageJsonFile != null) {
                Console.WriteLine("Copying custom package.json.");
                File.Copy(cmdcfg.PackageJsonFile, Path.Combine(cmdcfg.BuildPath, "package.json"), true);
            }

            var checkForNodeModulesDirPath = Path.Combine(cmdcfg.BuildPath, "node_modules");
            if (Directory.Exists(checkForNodeModulesDirPath) == false || cmdcfg.ForceNpmInstall || cmdcfg.PackageJsonFile != null)

                Console.WriteLine("Start npm install...");
            ProcessHelper.CmdExecute("npm install --production", cmdcfg.BuildPath);
            Console.WriteLine("Start npm install electron-builder...");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // Works proper on Windows... 
                ProcessHelper.CmdExecute("npm install electron-builder --global", cmdcfg.BuildPath);
            }
            else {
                // ToDo: find another solution or document it proper
                // GH Issue https://github.com/electron-userland/electron-prebuilt/issues/48
                Console.WriteLine(
                    "Electron Packager - make sure you invoke 'sudo npm install electron-builder --global' at " +
                    cmdcfg.BuildPath + " manually. Sry.");
            }
            return true;
        }


        /// <summary> Builds the ElectronHostHook files. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool BuildElectronHostHook() {
            Console.WriteLine("ElectronHostHook handling started...");
            var electronhosthookDir = Path.Combine(Directory.GetCurrentDirectory(), "ElectronHostHook");
            if (Directory.Exists(electronhosthookDir)) {
                var hosthookDir = Path.Combine(cmdcfg.BuildPath, "ElectronHostHook");
                DirectoryCopy.Do(electronhosthookDir, hosthookDir, true, new List<string>() {"node_modules"});

                Console.WriteLine("Start npm install for hosthooks...");
                ProcessHelper.CmdExecute("npm install --production", hosthookDir);

                // ToDo: Global TypeScript installation is needed for ElectronHostHook
                //string tscPath = Path.Combine(tempPath, "node_modules", ".bin");

                // ToDo: Not sure if this runs under linux/macos
                ProcessHelper.CmdExecute(@"tsc -p . --sourceMap false", electronhosthookDir);
            }

            return true;
        }

        /// <summary> Builds the application. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool BuildApp() {
            Console.WriteLine("Build Electron Desktop Application...");

            Console.WriteLine("Executing electron magic in this directory: " + buildPath);

            // ToDo: Make the same thing easer with native c# - we can save a tmp file in production code :)
            Console.WriteLine("Create electron-builder configuration file...");
            ProcessHelper.CmdExecute($"node build-helper.js", cmdcfg.BuildPath);


            // ToDo: Need a solution for --asar support
            Console.WriteLine($"Package Electron App for Platform {cmdcfg.ElectronPackerPlatform}...");
            ProcessHelper.CmdExecute(
                $"electron-builder . --config=./bin/electron-builder.json --platform={cmdcfg.ElectronPackerPlatform} --arch={cmdcfg.ElectronArch} {cmdcfg.ElectronParams} --out=\"{packagePath}\" --overwrite", 
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
            Console.WriteLine($"Build ASP.NET Core App for {cmdcfg.RuntimeIdentifier}...");
            Console.WriteLine($"Executing dotnet publish in this directory: {cmdcfg.BuildPath}");
            var tempBinPath = Path.Combine(cmdcfg.BuildPath, "bin");
            Console.WriteLine(
                $"Build ASP.NET Core App for {cmdcfg.RuntimeIdentifier} under {cmdcfg.DotnetConfiguration}-Configuration...");
            var resultCode = ProcessHelper.CmdExecute(
                $"dotnet publish -r {cmdcfg.RuntimeIdentifier} -c {cmdcfg.DotnetConfiguration} --output \"{tempBinPath}\"",
                cmdcfg.ProjectPath);
            if (resultCode != 0) {
                Console.WriteLine($"Error occurred during dotnet publish: {resultCode}");
                return false;
            }
            return true;
        }



        /// <summary> Setup the node modules directory. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool SetupNodeModules() {

            DeployEmbeddedElectronFiles.Do(cmdcfg.BuildPath);
            var nodeModulesDirPath = Path.Combine(cmdcfg.BuildPath, "node_modules");
            if (cmdcfg.PackageJsonFile != null) {
                Console.WriteLine("Copying custom package.json.");
                File.Copy(cmdcfg.PackageJsonFile, Path.Combine(cmdcfg.BuildPath, "package.json"), true);
            }

            var checkForNodeModulesDirPath = Path.Combine(cmdcfg.BuildPath, "node_modules");
            if (Directory.Exists(checkForNodeModulesDirPath) == false || cmdcfg.ForceNpmInstall || cmdcfg.PackageJsonFile != null)

                Console.WriteLine("Start npm install...");
            ProcessHelper.CmdExecute("npm install --production", cmdcfg.BuildPath);
            Console.WriteLine("Start npm install electron-packager...");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // Works proper on Windows... 
                ProcessHelper.CmdExecute("npm install electron-packager --global", cmdcfg.BuildPath);
            }
            else {
                // ToDo: find another solution or document it proper
                // GH Issue https://github.com/electron-userland/electron-prebuilt/issues/48
                Console.WriteLine(
                    "Electron Packager - make sure you invoke 'sudo npm install electron-packager --global' at " +
                    cmdcfg.BuildPath + " manually. Sry.");
            }
            return true;
        }



        /// <summary> Builds the ElectronHostHook files. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool BuildElectronHostHook() {
            Console.WriteLine("ElectronHostHook handling started...");
            var electronhosthookDir = Path.Combine(Directory.GetCurrentDirectory(), "ElectronHostHook");
            if (Directory.Exists(electronhosthookDir)) {
                var hosthookDir = Path.Combine(cmdcfg.BuildPath, "ElectronHostHook");
                DirectoryCopy.Do(electronhosthookDir, hosthookDir, true, new List<string>() {"node_modules"});

                Console.WriteLine("Start npm install for hosthooks...");
                ProcessHelper.CmdExecute("npm install --production", hosthookDir);

                var tscPath = Path.Combine(cmdcfg.BuildPath, "node_modules", ".bin");
                // ToDo: Not sure if this runs under linux/macos
                ProcessHelper.CmdExecute(@"tsc -p ../../ElectronHostHook --sourceMap false", tscPath);
            }

            return true;
        }

        /// <summary> Builds the application. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool BuildApp() {
            Console.WriteLine("Build Electron Desktop Application...");

            // electron build directory
            var buildPath = Path.Combine(cmdcfg.BuildPath, "..");
            // make sure directory is absolute if relative
            buildPath = Path.GetFullPath(buildPath);

            Console.WriteLine("Executing electron magic in this directory: " + buildPath);
            // ToDo: Need a solution for --asar support
            Console.WriteLine($"Package Electron App for Platform {cmdcfg.ElectronPackerPlatform}...");
            ProcessHelper.CmdExecute(
                $"electron-packager . --platform={cmdcfg.ElectronPackerPlatform} --arch={cmdcfg.ElectronArch} {cmdcfg.ElectronParams} --out=\"{buildPath}\" --overwrite",
                cmdcfg.BuildPath);

            return true;
        }
    }
}
