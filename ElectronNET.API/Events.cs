using System;
using System.Globalization;
using Quobject.EngineIoClientDotNet.ComponentEmitter;

namespace ElectronNET.API
{
    /// <summary>
    /// Generic Event Consumers for Electron Modules
    /// </summary>
    internal class Events
    {
        private static Events _events;
        private static object _syncRoot = new object();
        private TextInfo _ti = new CultureInfo("en-US", false).TextInfo;
        private Events()
        {

        }

        public static Events Instance
        {
            get
            {
                if (_events == null)
                {
                    lock (_syncRoot)
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
        /// <param name="fn">The event handler</param>
        public void On(string moduleName, string eventName, Action fn)
            => On(moduleName, eventName, new ListenerImpl(fn));

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        public void On(string moduleName, string eventName, Action<object> fn)
            => On(moduleName, eventName, new ListenerImpl(fn));

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        private void On(string moduleName, string eventName, IListener fn)
        {
            var listener = $"{moduleName}{_ti.ToTitleCase(eventName)}Completed";
            var subscriber = $"register-{moduleName}-on-event";
            
            BridgeConnector.Socket.On(listener, fn);
            BridgeConnector.Socket.Emit(subscriber, eventName, listener);
        }

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        public void Once(string moduleName, string eventName, Action fn)
            => Once(moduleName, eventName, new ListenerImpl(fn));

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        public void Once(string moduleName, string eventName, Action<object> fn)
            => Once(moduleName, eventName, new ListenerImpl(fn));

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        private void Once(string moduleName, string eventName, IListener fn)
        {
            var listener = $"{moduleName}{_ti.ToTitleCase(eventName)}Completed";
            var subscriber = $"register-{moduleName}-once-event";
            BridgeConnector.Socket.Once(listener, fn);
            BridgeConnector.Socket.Emit(subscriber, eventName, listener);
        }

    }
}
