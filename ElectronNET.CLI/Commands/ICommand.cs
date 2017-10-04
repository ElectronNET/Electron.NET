using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands
{
    /// <summary>
    /// Interface for commands to implement.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// If enabled the tool will prompt for required fields if they are not already given.
        /// </summary>
        bool DisableInteractive { get; set; }

        Task<bool> ExecuteAsync();
    }
}
