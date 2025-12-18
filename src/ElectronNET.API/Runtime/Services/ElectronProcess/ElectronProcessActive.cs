namespace ElectronNET.Runtime.Services.ElectronProcess
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;

    /// <summary>
    /// Launches and manages the Electron app process.
    /// </summary>
    [Localizable(false)]
    internal class ElectronProcessActive : ElectronProcessBase
    {
        private readonly bool isUnpackaged;
        private readonly string electronBinaryName;
        private readonly string extraArguments;
        private readonly int socketPort;
        private ProcessRunner process;

        /// <summary>Initializes a new instance of the <see cref="ElectronProcessActive"/> class.</summary>
        /// <param name="isUnpackaged">The is debug.</param>
        /// <param name="electronBinaryName">Name of the electron.</param>
        /// <param name="extraArguments">The extraArguments.</param>
        /// <param name="socketPort">The socket port.</param>
        public ElectronProcessActive(bool isUnpackaged, string electronBinaryName, string extraArguments, int socketPort)
        {
            this.isUnpackaged = isUnpackaged;
            this.electronBinaryName = electronBinaryName;
            this.extraArguments = extraArguments;
            this.socketPort = socketPort;
        }

        protected override async Task StartCore()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string startCmd, args, workingDir;

            if (this.isUnpackaged)
            {
                this.CheckRuntimeIdentifier();

                var electrondir = Path.Combine(dir.FullName, ".electron");

                ProcessRunner chmodRunner = null;

                try
                {
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        var distFolder = Path.Combine(electrondir, "node_modules", "electron", "dist");

                        chmodRunner = new ProcessRunner("ElectronRunner-Chmod");
                        chmodRunner.Run("chmod", "-R +x " + distFolder, electrondir);
                        await chmodRunner.WaitForExitAsync().ConfigureAwait(true);

                        if (chmodRunner.LastExitCode != 0)
                        {
                            throw new Exception("Failed to set executable permissions on Electron dist folder.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("[StartCore]: Exception: " + chmodRunner?.StandardError);
                    Console.Error.WriteLine("[StartCore]: Exception: " + chmodRunner?.StandardOutput);
                    Console.Error.WriteLine("[StartCore]: Exception: " + ex);
                }

                startCmd = Path.Combine(electrondir, "node_modules", "electron", "dist", "electron");

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    startCmd = Path.Combine(electrondir, "node_modules", "electron", "dist", "Electron.app", "Contents", "MacOS", "Electron");
                }

                args = $"main.js -unpackeddotnet --trace-warnings -electronforcedport={this.socketPort:D} " + this.extraArguments;
                workingDir = electrondir;
            }
            else
            {
                dir = dir.Parent!.Parent!;
                startCmd = Path.Combine(dir.FullName, this.electronBinaryName);
                args = $"-dotnetpacked -electronforcedport={this.socketPort:D} " + this.extraArguments;
                workingDir = dir.FullName;
            }

            // We don't await this in order to let the state transition to "Starting"
            Task.Run(async () => await this.StartInternal(startCmd, args, workingDir).ConfigureAwait(false));
        }

        private void CheckRuntimeIdentifier()
        {
            var buildInfoRid = ElectronNetRuntime.BuildInfo.RuntimeIdentifier;
            if (string.IsNullOrEmpty(buildInfoRid))
            {
                return;
            }

            var osPart = buildInfoRid.Split('-').First();

            var mismatch = false;

            switch (osPart)
            {
                case "win":

                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        mismatch = true;
                    }

                    break;

                case "linux":

                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        mismatch = true;
                    }

                    break;

                case "osx":

                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        mismatch = true;
                    }

                    break;

                case "freebsd":

                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                    {
                        mismatch = true;
                    }

                    break;
            }

            if (mismatch)
            {
                throw new PlatformNotSupportedException($"This Electron.NET application was built for '{buildInfoRid}'. It cannot run on this platform.");
            }
        }

        protected override Task StopCore()
        {
            this.process.Cancel();
            return Task.CompletedTask;
        }

        private async Task StartInternal(string startCmd, string args, string directoriy)
        {
            try
            {
                await Task.Delay(10.ms()).ConfigureAwait(false);

                Console.Error.WriteLine("[StartInternal]: startCmd: {0}", startCmd);
                Console.Error.WriteLine("[StartInternal]: args: {0}", args);

                this.process = new ProcessRunner("ElectronRunner");
                this.process.ProcessExited += this.Process_Exited;
                this.process.Run(startCmd, args, directoriy);

                await Task.Delay(500.ms()).ConfigureAwait(false);

                Console.Error.WriteLine("[StartInternal]: after run:");

                if (!this.process.IsRunning)
                {
                    Console.Error.WriteLine("[StartInternal]: Process is not running: " + this.process.StandardError);
                    Console.Error.WriteLine("[StartInternal]: Process is not running: " + this.process.StandardOutput);

                    Task.Run(() => this.TransitionState(LifetimeState.Stopped));

                    throw new Exception("Failed to launch the Electron process.");
                }

                this.TransitionState(LifetimeState.Ready);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[StartInternal]: Exception: " + this.process?.StandardError);
                Console.Error.WriteLine("[StartInternal]: Exception: " + this.process?.StandardOutput);
                Console.Error.WriteLine("[StartInternal]: Exception: " + ex);
                throw;
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            this.TransitionState(LifetimeState.Stopped);
        }
    }
}