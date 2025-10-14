namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Controls the behavior of <see cref="App.Relaunch(RelaunchOptions)"/>.
    /// </summary>
    public class RelaunchOptions
    {
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public string[] Args { get; set; }

        /// <summary>
        /// Gets or sets the execute path.
        /// </summary>
        /// <value>
        /// The execute path.
        /// </value>
        public string ExecPath { get; set; }
    }
}