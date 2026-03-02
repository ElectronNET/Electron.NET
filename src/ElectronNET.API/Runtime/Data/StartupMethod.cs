namespace ElectronNET.Runtime.Data
{
    public enum StartupMethod
    {
        /// <summary>Packaged Electron app where Electron launches the DotNet app.</summary>
        /// <remarks>
        ///     This is the classic way of ElectrronNET startup.
        /// </remarks>
        PackagedElectronFirst,

        /// <summary>Packaged Electron app where DotNet launches the Electron prozess.</summary>
        /// <remarks>
        ///     Provides better ways for managing the overall app lifecycle.
        ///     On the command lines, this is "dotnetpacked"
        /// </remarks>
        PackagedDotnetFirst,

        /// <summary>Unpackacked execution for debugging the Electron process and NodeJS.</summary>
        /// <remarks>
        ///     Similar to the legacy ElectronNET debugging but without packaging (=fast) and allows selection of
        ///     the debug adapter. It's rarely useful, unless it's about debugging NodeJS.
        ///     Note: 'Unpackaged' means that it's run directly from the compilation output folders (./bin/*).
        ///     On the command lines, this is "unpackedelectron"
        /// </remarks>
        UnpackedElectronFirst,


        /// <summary>Unpackacked execution for debugging the DotNet process.</summary>
        /// <remarks>
        ///     This is the new way of super-fast startup for debugging in-place with Hot Reload
        ///     (edit and continue), even on WSL - all from within Visual Studio.
        ///     Note: 'Unpackaged' means that it's run directly from the compilation output folders (./bin/*).
        ///     On the command lines, this is "unpackeddotnet"
        /// </remarks>
        UnpackedDotnetFirst,
    }
}