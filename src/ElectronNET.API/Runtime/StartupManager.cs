namespace ElectronNET.Runtime
{
    using System;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ElectronNET.Runtime.Controllers;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Helpers;

    internal class StartupManager
    {
        public void Initialize()
        {
            try
            {
                ElectronNetRuntime.BuildInfo = this.GatherBuildInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            this.CollectProcessData();
            this.SetElectronExecutable();


            ElectronNetRuntime.StartupMethod = this.DetectAppTypeAndStartup();
            Console.WriteLine((string)("Evaluated StartupMethod: " + ElectronNetRuntime.StartupMethod));

            if (ElectronNetRuntime.DotnetAppType != DotnetAppType.AspNetCoreApp)
            {
                ElectronNetRuntime.RuntimeControllerCore = this.CreateRuntimeController();
            }
        }

        private RuntimeControllerBase CreateRuntimeController()
        {
            switch (ElectronNetRuntime.StartupMethod)
            {
                case StartupMethod.PackagedDotnetFirst:
                case StartupMethod.UnpackedDotnetFirst:
                    return new RuntimeControllerDotNetFirst();
                case StartupMethod.PackagedElectronFirst:
                case StartupMethod.UnpackedElectronFirst:
                    return new RuntimeControllerElectronFirst();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private StartupMethod DetectAppTypeAndStartup()
        {
            var isLaunchedByDotNet = LaunchOrderDetector.CheckIsLaunchedByDotNet();
            var isUnPackaged = UnpackagedDetector.CheckIsUnpackaged();

            if (isLaunchedByDotNet)
            {
                if (isUnPackaged)
                {
                    return StartupMethod.UnpackedDotnetFirst;
                }

                return StartupMethod.PackagedDotnetFirst;
            }
            else
            {
                if (isUnPackaged)
                {
                    return StartupMethod.UnpackedElectronFirst;
                }

                return StartupMethod.PackagedElectronFirst;
            }
        }

        private void CollectProcessData()
        {
            var argsList = Environment.GetCommandLineArgs().ToImmutableList();

            ElectronNetRuntime.ProcessArguments = argsList;

            var portArg = argsList.FirstOrDefault(e => e.Contains(ElectronNetRuntime.ElectronPortArgumentName, StringComparison.OrdinalIgnoreCase));

            if (portArg != null)
            {
                var parts = portArg.Split('=', StringSplitOptions.TrimEntries);
                if (parts.Length > 1 && int.TryParse(parts[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result))
                {
                    ElectronNetRuntime.ElectronSocketPort = result;

                    Console.WriteLine("Use Electron Port: " + result);
                }
            }

            var pidArg = argsList.FirstOrDefault(e => e.Contains(ElectronNetRuntime.ElectronPidArgumentName, StringComparison.OrdinalIgnoreCase));

            if (pidArg != null)
            {
                var parts = pidArg.Split('=', StringSplitOptions.TrimEntries);
                if (parts.Length > 1 && int.TryParse(parts[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result))
                {
                    ElectronNetRuntime.ElectronProcessId = result;

                    Console.WriteLine("Electron Process ID: " + result);
                }
            }
        }

        private void SetElectronExecutable()
        {
            string executable = ElectronNetRuntime.BuildInfo.ElectronExecutable;
            if (string.IsNullOrEmpty(executable))
            {
                throw new Exception("AssemblyMetadataAttribute 'ElectronExecutable' could not be found!");
            }

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                executable += ".exe";
            }

            ElectronNetRuntime.ElectronExecutable = executable;
        }

        private BuildInfo GatherBuildInfo()
        {
            var buildInfo = new BuildInfo();

            var electronAssembly = Assembly.GetEntryAssembly();

            if (electronAssembly == null)
            {
                return buildInfo;
            }

            if (electronAssembly.GetName().Name == "testhost" || electronAssembly.GetName().Name == "ReSharperTestRunner")
            {
                electronAssembly = AppDomain.CurrentDomain.GetData("ElectronTestAssembly") as Assembly ?? electronAssembly;
            }

            var attributes = electronAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToList();

            if (attributes.Count > 0)
            {
                buildInfo.ElectronExecutable = attributes.FirstOrDefault(e => e.Key == nameof(buildInfo.ElectronExecutable))?.Value;
                buildInfo.ElectronVersion = attributes.FirstOrDefault(e => e.Key == nameof(buildInfo.ElectronVersion))?.Value;
                buildInfo.RuntimeIdentifier = attributes.FirstOrDefault(e => e.Key == nameof(buildInfo.RuntimeIdentifier))?.Value;
                buildInfo.Title = attributes.FirstOrDefault(e => e.Key == nameof(buildInfo.Title))?.Value;
                buildInfo.Version = attributes.FirstOrDefault(e => e.Key == nameof(buildInfo.Version))?.Value;
                buildInfo.BuildConfiguration = attributes.FirstOrDefault(e => e.Key == nameof(buildInfo.BuildConfiguration))?.Value;
                var isAspNet = attributes.FirstOrDefault(e => e.Key == "IsAspNet")?.Value;
                var isSingleInstance = attributes.FirstOrDefault(e => e.Key == nameof(buildInfo.ElectronSingleInstance))?.Value;
                var httpPort = attributes.FirstOrDefault(e => e.Key == "AspNetHttpPort")?.Value;

                if (isAspNet?.Length > 0 && bool.TryParse(isAspNet, out var isAspNetActive) && isAspNetActive)
                {
                    ElectronNetRuntime.DotnetAppType = DotnetAppType.AspNetCoreApp;
                }

                if (isSingleInstance?.Length > 0 && bool.TryParse(isSingleInstance, out var isSingleInstanceActive) && isSingleInstanceActive)
                {
                    buildInfo.ElectronSingleInstance = "yes";
                }
                else
                {
                    buildInfo.ElectronSingleInstance = "no";
                }

                if (httpPort?.Length > 0 && int.TryParse(httpPort, out var port))
                {
                    ElectronNetRuntime.AspNetWebPort = port;
                }
            }

            return buildInfo;
        }
    }
}