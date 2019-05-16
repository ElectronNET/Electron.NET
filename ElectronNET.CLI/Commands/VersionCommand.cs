using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands {

    /// <summary> Show version Command. </summary>
    public class VersionCommand : ICommand {

        /// <summary> Show version Command Execute. </summary>
        /// <returns> Show version Command Task. </returns>
        public Task<bool> ExecuteAsync() {
            return Task.Run(() => {
                var version = GetVersion();
                if (!string.IsNullOrEmpty(version))
                    Console.WriteLine($"Electron.NET Tools {version}");

                Console.WriteLine("Project Home: https://github.com/ElectronNET/Electron.NET");

                var fullversion = GetVersionFull();
                if (!string.IsNullOrEmpty(fullversion))
                    Console.WriteLine($"Full Version: {fullversion}");

                return true;
            });
        }

        /// <summary> Gets the version without the hash. </summary>
        /// <returns> The version. </returns>
        public static string GetVersion() {
            var runtimeVersion = typeof(VersionCommand)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>();
            return runtimeVersion.Version;
        }

        /// <summary> Gets full version including the hash. </summary>
        /// <returns> The full version. </returns>
        public static string GetVersionFull() {
            AssemblyInformationalVersionAttribute attribute = null;
            try {
                attribute = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            }
            catch (AmbiguousMatchException) {
                // Catch exception and continue if multiple attributes are found.
            }
            return attribute?.InformationalVersion;
        }

    }
}
