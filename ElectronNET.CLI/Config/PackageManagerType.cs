using System;

namespace ElectronNET.CLI.Config {

    /// <summary> The type of package manager to use. </summary>
    public enum PackageManagerType {

        /// <summary> npm - The original Node Package Manager. </summary>
        npm,

        /// <summary> yarn package manager. </summary>
        yarn,

        /// <summary> pnpm package manager. Similar to npm but saves disk space by using links on the disk </summary>
        pnpm

    }

    /// <summary> Extension methods for PackageManagerType. </summary>
    public static class PackageManagerTypeExtensions {

        /// <summary> Determine the install command to use for the package manager. </summary>
        /// <param name="type"> The type to act on. </param>
        /// <returns> The install command to use. </returns>
        public static string ToInstallCmd(this PackageManagerType type) {
            if (type == PackageManagerType.npm)
                return "npm install";
            if (type == PackageManagerType.yarn)
                return "yarn install";
            if (type == PackageManagerType.pnpm)
                return "pnpm install";
            return null;
        }
    }
}
