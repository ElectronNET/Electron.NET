using System.Threading.Tasks;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Allows you to execute native JavaScript/TypeScript code from the host process.
    /// 
    /// It is only possible if the Electron.NET CLI has previously added an 
    /// ElectronHostHook directory:
    /// <c>electronize add HostHook</c>
    /// </summary>
    public interface IHostHook
    {
        /// <summary>
        /// Execute native JavaScript/TypeScript code.
        /// </summary>
        /// <param name="socketEventName">Socket name registered on the host.</param>
        /// <param name="arguments">Optional parameters.</param>
        void Call(string socketEventName, params dynamic[] arguments);

        /// <summary>
        /// Execute native JavaScript/TypeScript code.
        /// </summary>
        /// <typeparam name="T">Results from the executed host code.</typeparam>
        /// <param name="socketEventName">Socket name registered on the host.</param>
        /// <param name="arguments">Optional parameters.</param>
        /// <returns></returns>
        Task<T> CallAsync<T>(string socketEventName, params dynamic[] arguments);
    }
}