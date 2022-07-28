using System;
using System.Globalization;

namespace ElectronNET.API
{
    /// <summary>
    /// Generic Event Consumers for Electron Modules
    /// </summary>
    internal class Events
    {
        private static Events _events;
        private static readonly object _syncRoot = new();
        private readonly TextInfo _ti = new CultureInfo("en-US", false).TextInfo;
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
        public void On(string moduleName, string eventName, Action fn) => On(moduleName, eventName, _ => fn());


        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        public void On(string moduleName, string eventName, Action<object> fn)
        {
            var listener = $"{moduleName}{_ti.ToTitleCase(eventName)}Completed";
            var subscriber = $"register-{moduleName}-on-event";
            
            BridgeConnector.On(listener, fn);
            BridgeConnector.Emit(subscriber, eventName, listener);
        }

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        public void Once(string moduleName, string eventName, Action fn) => Once(moduleName, eventName, _ => fn());

        /// <summary>
        /// Subscribe to an unmapped electron event.
        /// </summary>
        /// <param name="moduleName">The name of the module, e.g. app, dock, etc...</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="fn">The event handler</param>
        public void Once(string moduleName, string eventName, Action<object> fn)
        {
            var listener = $"{moduleName}{_ti.ToTitleCase(eventName)}Completed";
            var subscriber = $"register-{moduleName}-once-event";
            BridgeConnector.Once(listener, fn);
            BridgeConnector.Emit(subscriber, eventName, listener);
        }
    }
}
