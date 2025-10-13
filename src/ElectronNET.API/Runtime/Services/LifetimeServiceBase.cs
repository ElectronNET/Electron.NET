namespace ElectronNET.Runtime.Services
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using ElectronNET.Runtime.Data;

    public abstract class LifetimeServiceBase
    {
        private readonly TaskCompletionSource tcsStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource tcsReady = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource tcsStopping = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource tcsStopped = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        private LifetimeState state = LifetimeState.Uninitialized;

        protected LifetimeServiceBase() { }

        public event EventHandler Starting;

        public event EventHandler Started;

        public event EventHandler Ready;

        public event EventHandler Stopping;

        public event EventHandler Stopped;


        public LifetimeState State => this.state;

        public Task WaitStartedTask => this.tcsStarted.Task;

        public Task WaitReadyTask => this.tcsReady.Task;

        public Task WaitStoppingTask => this.tcsStopping.Task;

        public Task WaitStoppedTask => this.tcsStopped.Task;


        public virtual async Task Start()
        {
            this.ValidateMaxState(LifetimeState.Uninitialized);

            await this.StartCore().ConfigureAwait(false);

            this.TransitionState(LifetimeState.Starting);
        }

        public virtual async Task Stop()
        {
            this.ValidateMaxState(LifetimeState.Ready);

            await this.StopCore().ConfigureAwait(false);

            this.TransitionState(LifetimeState.Stopping);
        }

        protected virtual Task StopCore()
        {
            return Task.CompletedTask;
        }

        protected virtual Task StartCore()
        {
            return Task.CompletedTask;
        }

        private void ValidateMaxState(LifetimeState evalState, [CallerMemberName] string callerMemberName = null)
        {
            if (this.state > evalState)
            {
                throw new Exception($"Invalid state! Cannot execute {callerMemberName} in state {this.state}");
            }
        }

        protected void TransitionState(LifetimeState newState)
        {
            if (newState == this.state)
            {
                return;
            }

            if (newState < this.state)
            {
                throw new Exception($"Invalid state transision from {this.state} to {newState}: " + this.GetType().Name);
            }

            var oldState = this.state;

            this.state = newState;

            switch (this.state)
            {
                case LifetimeState.Starting:
                    this.Starting?.Invoke(this, EventArgs.Empty);
                    break;
                case LifetimeState.Started:
                    this.Started?.Invoke(this, EventArgs.Empty);
                    break;
                case LifetimeState.Ready:
                    this.Ready?.Invoke(this, EventArgs.Empty);
                    break;
                case LifetimeState.Stopping:
                    this.Stopping?.Invoke(this, EventArgs.Empty);
                    break;
                case LifetimeState.Stopped:
                    this.Stopped?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (oldState < LifetimeState.Started && newState >= LifetimeState.Started)
            {
                this.tcsStarted.TrySetResult();
            }

            if (oldState < LifetimeState.Ready && newState >= LifetimeState.Ready)
            {
                this.tcsReady.TrySetResult();
            }

            if (oldState < LifetimeState.Stopping && newState >= LifetimeState.Stopping)
            {
                this.tcsStopping.TrySetResult();
            }

            if (oldState < LifetimeState.Stopped && newState >= LifetimeState.Stopped)
            {
                this.tcsStopped.TrySetResult();
            }
        }
    }
}
