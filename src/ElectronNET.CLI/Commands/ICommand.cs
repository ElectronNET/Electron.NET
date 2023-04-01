using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands
{
    /// <summary>
    /// Interface for commands to implement.
    /// </summary>
    public interface ICommand
    {
        Task<bool> ExecuteAsync();
    }
}
