namespace ElectronNET.Runtime
{
    using System;
    using System.Threading.Tasks;
    using ElectronNET.Runtime.Data;

    public interface IElectronNetRuntimeController
    {
        LifetimeState State { get; }

        Task WaitStartedTask { get; }

        Task WaitReadyTask { get; }

        Task WaitStoppingTask { get; }

        Task WaitStoppedTask { get; }

        event EventHandler Starting;

        event EventHandler Started;

        event EventHandler Ready;

        event EventHandler Stopping;

        event EventHandler Stopped;

        Task Start();

        Task Stop();
    }
}