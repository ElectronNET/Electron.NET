using System;
using System.Threading.Tasks;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Detect keyboard events when the application does not have keyboard focus.
    /// </summary>
    public interface IGlobalShortcut
    {
        /// <summary>
        /// Registers a global shortcut of accelerator. 
        /// The callback is called when the registered shortcut is pressed by the user.
        /// 
        /// When the accelerator is already taken by other applications, this call will 
        /// silently fail.This behavior is intended by operating systems, since they don’t
        /// want applications to fight for global shortcuts.
        /// </summary>
        void Register(string accelerator, Action function);

        /// <summary>
        /// When the accelerator is already taken by other applications, 
        /// this call will still return false. This behavior is intended by operating systems,
        /// since they don’t want applications to fight for global shortcuts.
        /// </summary>
        /// <returns>Whether this application has registered accelerator.</returns>
        Task<bool> IsRegisteredAsync(string accelerator);

        /// <summary>
        /// Unregisters the global shortcut of accelerator.
        /// </summary>
        void Unregister(string accelerator);

        /// <summary>
        /// Unregisters all of the global shortcuts.
        /// </summary>
        void UnregisterAll();
    }
}