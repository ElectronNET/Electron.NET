namespace ElectronNET.Common
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class encapsulating out-of-process execution of console applications.
    /// </summary>
    /// <remarks>
    ///     Why this class?
    ///     Probably everybody who has tried to use System.Diagnotics.Process cross-platform and with reading
    ///     stderr and stdout will know that it is a pretty quirky API.
    ///     The code below may look weird, even non-sensical, but it works 100% reliable with all .net frameworks
    ///     and .net versions and on every platform where .net runs. This is just the innermost core, that's why
    ///     there are many dead ends, but it has all the crucial parts.
    /// </remarks>
    /// <seealso cref="IDisposable" />
    [SuppressMessage("ReSharper", "SuspiciousLockOverSynchronizationPrimitive")]
    public class ProcessRunner : IDisposable
    {
        private volatile Process process;
        private readonly StringBuilder stdOut = new StringBuilder(4 * 1024);
        private readonly StringBuilder stdErr = new StringBuilder(4 * 1024);

        private volatile ManualResetEvent stdOutEvent;
        private volatile ManualResetEvent stdErrEvent;
        private volatile Stopwatch stopwatch;

        /// <summary>Initializes a new instance of the <see cref="ProcessRunner" /> class.</summary>
        /// <param name="name">A name identifying the process to execute.</param>
        public ProcessRunner(string name)
        {
            this.Name = name;
        }

        public event EventHandler<EventArgs> ProcessExited;

        public bool IsDisposed { get; private set; }

        private Process Process
        {
            get
            {
                return this.process;
            }
        }

        public bool IsRunning
        {
            get
            {
                var proc = this.process;
                if (proc != null)
                {
                    try
                    {
                        return !proc.HasExited;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }
        }

        /// <summary>Gets the name identifying the process.</summary>
        /// <value>The name.</value>
        public string Name { get; }

        public string CommandLine { get; private set; }

        public string ExecutableFileName { get; private set; }

        public string WorkingFolder { get; private set; }

        public bool RecordStandardOutput { get; set; }

        public bool RecordStandardError { get; set; }

        public string StandardOutput
        {
            get
            {
                lock (this.stdOut)
                {
                    return this.stdOut.ToString();
                }
            }
        }

        public string StandardError
        {
            get
            {
                lock (this.stdErr)
                {
                    return this.stdErr.ToString();
                }
            }
        }

        public int? LastExitCode { get; private set; }

        public bool Run(string exeFileName, string commandLineArgs, string workingDirectory)
        {
            this.CommandLine = commandLineArgs;
            this.WorkingFolder = workingDirectory;
            this.ExecutableFileName = exeFileName;

            var startInfo = new RunnerParams(exeFileName)
            {
                Arguments = commandLineArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                ErrorDialog = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            return this.Run(startInfo);
        }

        protected bool Run(RunnerParams runnerParams)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }

            this.Close();

            this.LastExitCode = null;

            lock (this.stdOut)
            {
                this.stdOut.Clear();
            }

            lock (this.stdErr)
            {
                this.stdErr.Clear();
            }

            this.stdOutEvent = new ManualResetEvent(false);
            this.stdErrEvent = new ManualResetEvent(false);

            if (!this.OnBeforeStartProcessCore(runnerParams))
            {
                return false;
            }

            var startInfo = new ProcessStartInfo(runnerParams.FileName)
            {
                Arguments = runnerParams.Arguments,
                UseShellExecute = runnerParams.UseShellExecute,
                RedirectStandardOutput = runnerParams.RedirectStandardOutput,
                RedirectStandardError = runnerParams.RedirectStandardError,
                RedirectStandardInput = runnerParams.RedirectStandardInput,
                ErrorDialog = runnerParams.ErrorDialog,
                CreateNoWindow = runnerParams.CreateNoWindow,
                WorkingDirectory = runnerParams.WorkingDirectory
            };

            foreach (var variableSetting in runnerParams.EnvironmentVariables)
            {
                startInfo.EnvironmentVariables[variableSetting.Key] = variableSetting.Value;
            }

            var proc = new Process { StartInfo = startInfo };

            proc.EnableRaisingEvents = true;

            this.RegisterProcessEvents(proc);

            this.process = proc;

            try
            {
                this.process.Start();
                this.stopwatch = Stopwatch.StartNew();
                this.process.BeginOutputReadLine();
                this.process.BeginErrorReadLine();
                this.process.Refresh();
                this.OnProcessStartedCore();
            }
            catch (Exception ex)
            {
                this.OnProcessErrorCore(ex);
                this.Close();
                throw;
            }

            return true;
        }

        public async Task<bool> WriteAsync(string data)
        {
            var proc = this.Process;
            if (proc != null && !proc.HasExited)
            {
                try
                {
                    await proc.StandardInput.WriteAsync(data).ConfigureAwait(false);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}.{1}: {2}", ex, nameof(ProcessRunner), nameof(this.WriteAsync));
                }
            }

            return false;
        }

        public bool WaitForExit()
        {
            var proc = this.process;

            if (proc == null)
            {
                return true;
            }

            try
            {
                // Wait for process and all I/O to finish.
                proc.WaitForExit();
                return true;
            }
            catch (Exception ex)
            {
                this.OnProcessErrorCore(ex);
                return false;
            }
        }

        /// <summary>Sychronously waits for the specified amount and ends the process afterwards.</summary>
        /// <param name="timeoutMs">The timeout ms.</param>
        /// <remarks>This method allows for a clean exit, since it also waits until the StandardOutput and
        /// StandardError pipes are processed to the end.</remarks>
        /// <returns>true, if the process has exited gracefully; false otherwise.</returns>
        public bool WaitAndKill(int timeoutMs)
        {
            var proc = this.process;

            if (proc == null)
            {
                return true;
            }

            try
            {
                if (timeoutMs <= 0)
                {
                    throw new ArgumentException("Argument must be greater then 0", nameof(timeoutMs));
                }

                // Timed waiting. We need to wait for I/O ourselves.
                if (!proc.WaitForExit(timeoutMs))
                {
                    this.Cancel();
                }

                // Wait for the I/O to finish.
                var waitMs = (int)(timeoutMs - this.stopwatch.ElapsedMilliseconds);
                waitMs = Math.Max(waitMs, 10);
                this.stdOutEvent?.WaitOne(waitMs);

                waitMs = (int)(timeoutMs - this.stopwatch.ElapsedMilliseconds);
                waitMs = Math.Max(waitMs, 10);
                return this.stdErrEvent?.WaitOne(waitMs) ?? false;
            }
            finally
            {
                // Cleanup.
                this.Cancel();
            }
        }

        /// <summary>Asynchronously waits for the specified amount and ends the process afterwards.</summary>
        /// <param name="timeoutMs">The timeout ms.</param>
        /// <remarks>Tjhis method performs the wait operation on a threadpool thread.
        /// Only recommended for short timeouts and situations where a synchronous call is undesired.</remarks>
        /// <returns>true, if the process has exited gracefully; false otherwise.</returns>
        public Task<bool> WaitAndKillAsync(int timeoutMs)
        {
            var task = Task.Run(() => this.WaitAndKill(timeoutMs));
            return task;
        }

        /// <summary>Waits asynchronously for the process to exit.</summary>
        /// <param name="timeoutMs">The timeout ms.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>true, if the process has exited, false if the process is still running.</returns>
        /// <remarks>
        /// This methods waits until the process has existed or the
        /// <paramref name="timeoutMs" /> has elapsed.
        /// This method does not end the process itself.
        /// </remarks>
        public Task<bool> WaitForExitAsync(int timeoutMs, CancellationToken cancellationToken = default)
        {
            timeoutMs = Math.Max(0, timeoutMs);

            var timeoutSource = new CancellationTokenSource(timeoutMs);
            var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken);

            return this.WaitForExitAsync(linkedSource.Token);
        }

        /// <summary>Waits asynchronously for the process to exit.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <remarks>This methods waits until the process has existed or the
        /// <paramref name="cancellationToken"/> has been triggered.
        /// This method does not end the process itself.</remarks>
        /// <returns>true, if the process has exited, false if the process is still running.</returns>
        public async Task<bool> WaitForExitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var proc = this.process;

            if (proc == null)
            {
                return true;
            }

            var tcs = new TaskCompletionSource<bool>();

            // Use local function instead of a lambda to allow proper deregistration of the event
            void ProcessExited(object sender, EventArgs e)
            {
                tcs.TrySetResult(true);
            }

            try
            {
                proc.EnableRaisingEvents = true;
                proc.Exited += ProcessExited;

                if (proc.HasExited)
                {
                    return true;
                }

                using (cancellationToken.Register(() => tcs.TrySetResult(false)))
                {
                    return await tcs.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                proc.Exited -= ProcessExited;
            }
        }

        public void Cancel()
        {
            var proc = this.process;

            if (proc != null)
            {
                try
                {
                    // Invalidate cached data to requery.
                    proc.Refresh();

                    // We need to do this in case of a non-UI proc
                    // or one to be forced to cancel.
                    if (!proc.HasExited)
                    {
                        // Cancel all pending IO ops.
                        proc.CancelErrorRead();
                        proc.CancelOutputRead();
                    }

                    if (!proc.HasExited)
                    {
                        proc.Kill();
                    }
                }
                catch
                {
                    // Kill will throw when/if the process has already exited.
                }
            }

            var outEvent = this.stdOutEvent;
            this.stdOutEvent = null;
            if (outEvent != null)
            {
                lock (outEvent)
                {
                    outEvent.Close();
                    outEvent.Dispose();
                }
            }

            var errEvent = this.stdErrEvent;
            this.stdErrEvent = null;
            if (errEvent != null)
            {
                lock (errEvent)
                {
                    errEvent.Close();
                    errEvent.Dispose();
                }
            }
        }

        private void Close()
        {
            this.Cancel();

            var proc = this.process;
            this.process = null;
            if (proc != null)
            {
                try
                {
                    this.UnRegisterProcessEvents(proc);

                    // Dispose in all cases.
                    proc.Close();
                    proc.Dispose();
                }
                catch (Exception ex)
                {
                    this.OnProcessErrorCore(ex);
                }
            }
        }

        protected virtual void OnDispose()
        {
        }

        void IDisposable.Dispose()
        {
            this.IsDisposed = true;
            this.Close();
            this.OnDispose();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} {2}", this.GetType().Name, this.Name, this.process);
        }

        protected virtual bool OnBeforeStartProcessCore(RunnerParams processRunnerInfo)
        {
            return true;
        }

        protected virtual void OnProcessStartedCore()
        {
        }

        protected virtual void OnProcessErrorCore(Exception processException)
        {
        }

        protected virtual void OnProcessExitCore(int exitCode)
        {
        }

        private void RegisterProcessEvents(Process proc)
        {
            proc.ErrorDataReceived += this.Process_ErrorDataReceived;
            proc.OutputDataReceived += this.Process_OutputDataReceived;
            proc.Exited += this.Process_Exited;
        }

        private void UnRegisterProcessEvents(Process proc)
        {
            proc.ErrorDataReceived -= this.Process_ErrorDataReceived;
            proc.OutputDataReceived -= this.Process_OutputDataReceived;
            proc.Exited -= this.Process_Exited;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            this.WaitForExitAfterExited();
            this.SetExitCode();
            this.OnProcessExitCore(this.LastExitCode ?? -9998);
            this.ProcessExited?.Invoke(this, new EventArgs());
        }

        private void WaitForExitAfterExited()
        {
            try
            {
                // This shouldn't throw here, but the mono process implementation doesn't always behave as it should.
                this.process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when calling WaitForExit after exited event has fired: {0}.{1}: {2}", ex, nameof(ProcessRunner), nameof(this.WaitForExitAfterExited));
            }
        }

        private void SetExitCode()
        {
            int exitCode = -9999;

            try
            {
                if (this.Process != null)
                {
                    exitCode = this.Process.ExitCode;
                }
            }
            catch
            {
                // Ignore error.
            }

            this.LastExitCode = exitCode;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (this.RecordStandardError)
            {
                lock (this.stdErr)
                {
                    this.stdErr.AppendLine(e.Data);
                }
            }

            if (e.Data != null)
            {
                Console.WriteLine("|| " + e.Data);
            }
            else
            {
                var evt = this.stdErrEvent;
                if (evt != null)
                {
                    lock (evt)
                    {
                        try
                        {
                            evt.Set();
                        }
                        catch
                        {
                            // Ignore error.
                        }
                    }
                }
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (this.RecordStandardOutput)
            {
                lock (this.stdOut)
                {
                    this.stdOut.AppendLine(e.Data);
                }
            }

            if (e.Data != null)
            {
                Console.WriteLine("|| " + e.Data);
            }
            else
            {
                var evt = this.stdOutEvent;
                if (evt != null)
                {
                    lock (evt)
                    {
                        try
                        {
                            evt.Set();
                        }
                        catch
                        {
                            // Ignore error.
                        }
                    }
                }
            }
        }
    }
}