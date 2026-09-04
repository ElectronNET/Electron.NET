namespace ElectronNET.Runtime.Helpers
{
    using System;
    using System.IO;

    /// <summary>
    /// Resolves the directory that contains the Electron binary of a packaged application.
    /// </summary>
    internal static class ElectronRootDirResolver
    {
        /// <summary>The location of the Electron binary relative to the .NET application directory in the
        /// default (Electron-first) layout, where the .NET application is placed in 'resources/bin'.</summary>
        internal const string DefaultElectronRootDir = "../..";

        /// <summary>Resolves the Electron root directory relative to the given .NET application directory.</summary>
        /// <param name="baseDirectory">The directory containing the .NET application.</param>
        /// <returns>The directory containing the Electron binary.</returns>
        public static DirectoryInfo Resolve(DirectoryInfo baseDirectory)
        {
            var rootDir = ElectronNetRuntime.BuildInfo?.ElectronRootDir;

            if (string.IsNullOrWhiteSpace(rootDir))
            {
                rootDir = DefaultElectronRootDir;
            }

            // An absolute ElectronRootDir is used as-is by Path.Combine.
            return new DirectoryInfo(Path.GetFullPath(Path.Combine(baseDirectory.FullName, rootDir)));
        }
    }
}
