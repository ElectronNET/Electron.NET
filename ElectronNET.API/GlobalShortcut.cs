using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Detect keyboard events when the application does not have keyboard focus.
    /// </summary>
    public sealed class GlobalShortcut
    {
        private static GlobalShortcut _globalShortcut;
        private static readonly object _syncRoot = new();

        internal GlobalShortcut() { }

        internal static GlobalShortcut Instance
        {
            get
            {
                if (_globalShortcut == null)
                {
                    lock (_syncRoot)
                    {
                        if (_globalShortcut == null)
                        {
                            _globalShortcut = new GlobalShortcut();
                        }
                    }
                }

                return _globalShortcut;
            }
        }

        public Dictionary<string, Action> _shortcuts = new Dictionary<string, Action>();

        /// <summary>
        /// Registers a global shortcut of accelerator. 
        /// The callback is called when the registered shortcut is pressed by the user.
        /// 
        /// When the accelerator is already taken by other applications, this call will 
        /// silently fail.This behavior is intended by operating systems, since they don’t
        /// want applications to fight for global shortcuts.
        /// </summary>
        public void Register(string accelerator, Action function)
        {
            if (!_shortcuts.ContainsKey(accelerator))
            {
                _shortcuts.Add(accelerator, function);

                Electron.SignalrElectron.Clients.All.SendAsync("globalShortcut-register", accelerator);
            }
        }

        /// <summary>
        /// When the accelerator is already taken by other applications, 
        /// this call will still return false. This behavior is intended by operating systems,
        /// since they don’t want applications to fight for global shortcuts.
        /// </summary>
        /// <returns>Whether this application has registered accelerator.</returns>
        public async Task<bool> IsRegisteredAsync(string accelerator)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("globalShortcut-isRegistered", accelerator);
        }

        /// <summary>
        /// Unregisters the global shortcut of accelerator.
        /// </summary>
        public async void Unregister(string accelerator)
        {
            _shortcuts.Remove(accelerator);
            await Electron.SignalrElectron.Clients.All.SendAsync("globalShortcut-unregister", accelerator);
        }

        /// <summary>
        /// Unregisters all of the global shortcuts.
        /// </summary>
        public async void UnregisterAll()
        {
            _shortcuts.Clear();
            await Electron.SignalrElectron.Clients.All.SendAsync("globalShortcut-unregisterAll");
        }
    }
}