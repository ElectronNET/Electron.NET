#pragma warning disable IDE0130 // Namespace does not match folder structure
// ReSharper disable once CheckNamespace
namespace ElectronNET.API
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic Event Consumers for Electron Modules
    /// </summary>
    internal class Events
    {
        private static Events _events;
        private static readonly object SyncRoot = new();
        private readonly TextInfo _textInfo = new CultureInfo("en-US", false).TextInfo;

        private Events()
        { }

        public static Events Instance
        {
            get
            {
                if (_events == null)
                {
                    lock (SyncRoot)
                    {
                        if (_events == null)
                        {
                            _events = new Events();
                        }
                    }
                }

                return _events;
            }
        }

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="action">The event handler</param>
        public async Task On(string moduleName, string eventName, Action action)
        {
            var listener = $"{moduleName}{_textInfo.ToTitleCase(eventName)}Completed";
            var subscriber = $"register-{moduleName}-on-event";

            BridgeConnector.Socket.On(listener, action);
            await BridgeConnector.Socket.Emit(subscriber, eventName, listener).ConfigureAwait(false);
        }


        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="action">The event handler</param>
        public async Task On<T>(string moduleName, string eventName, Action<T> action)
        {
            var listener = $"{moduleName}{_textInfo.ToTitleCase(eventName)}Completed";
            var subscriber = $"register-{moduleName}-on-event";

            BridgeConnector.Socket.On(listener, action);
            await BridgeConnector.Socket.Emit(subscriber, eventName, listener).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        public async Task Once(string moduleName, string eventName, Action action)
        {
            var listener = $"{moduleName}{_textInfo.ToTitleCase(eventName)}Completed";
            var subscriber = $"register-{moduleName}-once-event";
            BridgeConnector.Socket.Once(listener, action);
            await BridgeConnector.Socket.Emit(subscriber, eventName, listener).ConfigureAwait(false);
        }


        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="action">The event handler</param>
        public async Task Once<T>(string moduleName, string eventName, Action<T> action)
        {
            var listener = $"{moduleName}{_textInfo.ToTitleCase(eventName)}Completed";
            var subscriber = $"register-{moduleName}-once-event";
            BridgeConnector.Socket.Once(listener, action);
            await BridgeConnector.Socket.Emit(subscriber, eventName, listener).ConfigureAwait(false);
        }

    }
}
