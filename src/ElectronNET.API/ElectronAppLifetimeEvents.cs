namespace ElectronNET
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents callbacks that are invoked during different phases of the Electron application
    /// lifetime. These callbacks allow consumers to hook into the startup sequence more
    /// granularly than the existing single callback. Callbacks return <see cref="Task"/>
    /// enabling asynchronous work to be awaited before the next phase of the Electron runtime
    /// commences.
    /// </summary>
    public class ElectronAppLifetimeEvents
    {
        /// <summary>
        /// Gets or sets the callback that is invoked once the Electron process and socket bridge
        /// have been established but before the Electron <c>ready</c> event has been
        /// acknowledged. Use this hook to register custom protocols or perform other
        /// initialization that must occur prior to the <c>ready</c> event.
        /// </summary>
        public Func<Task> OnBeforeReady { get; set; } = () => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the callback that is invoked when the Electron <c>ready</c> event is
        /// fired. Use this hook to create browser windows or perform post-ready initialization.
        /// </summary>
        public Func<Task> OnReady { get; set; } = () => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the callback that is invoked when the Electron process is about to quit.
        /// This maps to the <c>will-quit</c> event in Electron and can be used to perform
        /// graceful shutdown logic. The default implementation does nothing.
        /// </summary>
        public Func<Task> OnWillQuit { get; set; } = () => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the callback that is invoked when the Electron process has fully
        /// terminated. This can be used for cleanup tasks. The default implementation does
        /// nothing.
        /// </summary>
        public Func<Task> OnQuit { get; set; } = () => Task.CompletedTask;
    }
}