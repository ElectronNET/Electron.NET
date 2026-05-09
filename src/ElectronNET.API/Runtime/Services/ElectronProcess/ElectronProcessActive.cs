namespace ElectronNET.Runtime.Services.ElectronProcess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using ElectronNET.Common;
    using ElectronNET.Runtime.Data;

    /// <summary>
    /// Launches and manages the Electron app process.
    /// </summary>
    [Localizable(false)]
    internal class ElectronProcessActive : ElectronProcessBase
    {
        private const string AuthTokenEnvVar = "ELECTRONNET_AUTH_TOKEN";
        private const string StartupInfoEnvVar = "ELECTRONNET_STARTUP_INFO";

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

            // Generate the auth token on the .NET side (256 bit entropy) and pass it
            // to Electron via an environment variable. Electron will report the
            // OS-selected port via a temporary handshake file - this avoids any
            // dependency on parsing Electron's console output.
            var authToken = CreateAuthToken();
            var startupInfoPath = Path.Combine(
                Path.GetTempPath(),
                $"electronnet-startup-{Environment.ProcessId}-{Guid.NewGuid():N}.json");

            // We don't await this in order to let the state transition to "Starting"
            Task.Run(async () => await this.StartInternal(startCmd, args, workingDir, authToken, startupInfoPath).ConfigureAwait(false));
        }

        private static string CreateAuthToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToHexString(bytes).ToLowerInvariant();
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

        private async Task StartInternal(string startCmd, string args, string directoriy, string authToken, string startupInfoPath)
        {
            var tcs = new TaskCompletionSource();
            using var cts = new CancellationTokenSource(2 * 60_000); // cancel after 2 minutes
            using var _ = cts.Token.Register(() =>
            {
                // Time is over - let's kill the process and move on
                this.process.Cancel();
                // We don't want to raise exceptions here - just pass the barrier
                tcs.TrySetResult();
            });

            void Monitor_SocketIO_Failure(object sender, EventArgs e)
            {
                // We don't want to raise exceptions here - just pass the barrier
                if (tcs.Task.IsCompleted)
                {
                    this.Process_Exited(sender, e);
                }
                else
                {
                    tcs.TrySetResult();
                }
            }

            try
            {
                Console.Error.WriteLine("[StartInternal]: startCmd: {0}", startCmd);
                Console.Error.WriteLine("[StartInternal]: args: {0}", args);

                this.process = new ProcessRunner("ElectronRunner");
                this.process.ProcessExited += Monitor_SocketIO_Failure;

                var env = new Dictionary<string, string>
                {
                    [AuthTokenEnvVar] = authToken,
                    [StartupInfoEnvVar] = startupInfoPath,
                };

                this.process.Run(startCmd, args, directoriy, env);

                // Wait for Electron to write the startup-info file (or for the process to die / timeout).
                var waitTask = WaitForStartupInfoAsync(startupInfoPath, cts.Token);
                var completed = await Task.WhenAny(waitTask, tcs.Task).ConfigureAwait(false);

                int port = 0;
                if (completed == waitTask && waitTask.Status == TaskStatus.RanToCompletion)
                {
                    port = waitTask.Result;
                }

                Console.Error.WriteLine("[StartInternal]: after run:");

                if (!this.process.IsRunning)
                {
                    Console.Error.WriteLine("[StartInternal]: Process is not running: " + this.process.StandardError);
                    Console.Error.WriteLine("[StartInternal]: Process is not running: " + this.process.StandardOutput);

                    Task.Run(() => this.TransitionState(LifetimeState.Stopped));
                }
                else if (port > 0)
                {
                    ElectronNetRuntime.ElectronAuthToken = authToken;
                    ElectronNetRuntime.ElectronSocketPort = port;
                    this.TransitionState(LifetimeState.Ready);
                }
                else
                {
                    Console.Error.WriteLine("[StartInternal]: Did not receive Electron startup info before process exit/timeout.");
                    Task.Run(() => this.TransitionState(LifetimeState.Stopped));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[StartInternal]: Exception: " + this.process?.StandardError);
                Console.Error.WriteLine("[StartInternal]: Exception: " + this.process?.StandardOutput);
                Console.Error.WriteLine("[StartInternal]: Exception: " + ex);
                throw;
            }
            finally
            {
                try
                {
                    if (File.Exists(startupInfoPath))
                    {
                        File.Delete(startupInfoPath);
                    }
                }
                catch
                {
                    // best effort cleanup
                }
            }
        }

        private static async Task<int> WaitForStartupInfoAsync(string startupInfoPath, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (File.Exists(startupInfoPath))
                    {
                        var json = await File.ReadAllTextAsync(startupInfoPath, cancellationToken).ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            using var doc = JsonDocument.Parse(json);
                            if (doc.RootElement.TryGetProperty("port", out var portElement) &&
                                portElement.TryGetInt32(out var port) &&
                                port > 0)
                            {
                                return port;
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    // File may be partially written / racing with the writer - retry.
                }
                catch (IOException)
                {
                    // Same - transient races on file access; retry.
                }

                try
                {
                    await Task.Delay(50, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            return 0;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            this.TransitionState(LifetimeState.Stopped);
        }
    }
}