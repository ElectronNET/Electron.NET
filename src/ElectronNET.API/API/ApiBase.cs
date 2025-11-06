namespace ElectronNET.API
{
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using ElectronNET.Common;

    public abstract class ApiBase
    {
        internal const int PropertyTimeout = 1000;

        private readonly string objectName;
        private readonly ConcurrentDictionary<string, string> methodMessageNames = new ConcurrentDictionary<string, string>();

        public virtual int Id
        {
            get
            {
                return -1;
            }

            // ReSharper disable once ValueParameterNotUsed
            protected set
            {
            }
        }

        protected abstract string SocketEventCompleteSuffix { get; }

        protected ApiBase()
        {
            this.objectName = this.GetType().Name.LowerFirst();
        }

        protected void CallMethod0([CallerMemberName] string callerName = null)
        {
            var messageName = this.methodMessageNames.GetOrAdd(callerName, s => this.objectName + s);
            if (this.Id >= 0)
            {
                BridgeConnector.Socket.Emit(messageName, this.Id);
            }
            else
            {
                BridgeConnector.Socket.Emit(messageName);
            }
        }

        protected void CallMethod1(object val1, [CallerMemberName] string callerName = null)
        {
            var messageName = this.methodMessageNames.GetOrAdd(callerName, s => this.objectName + s);
            if (this.Id >= 0)
            {
                BridgeConnector.Socket.Emit(messageName, this.Id, val1);
            }
            else
            {
                BridgeConnector.Socket.Emit(messageName, val1);
            }
        }

        protected void CallMethod2(object val1, object val2, [CallerMemberName] string callerName = null)
        {
            var messageName = this.methodMessageNames.GetOrAdd(callerName, s => this.objectName + s);
            if (this.Id >= 0)
            {
                BridgeConnector.Socket.Emit(messageName, this.Id, val1, val2);
            }
            else
            {
                BridgeConnector.Socket.Emit(messageName, val1, val2);
            }
        }

        protected void CallMethod3(object val1, object val2, object val3, [CallerMemberName] string callerName = null)
        {
            var messageName = this.methodMessageNames.GetOrAdd(callerName, s => this.objectName + s);
            if (this.Id >= 0)
            {
                BridgeConnector.Socket.Emit(messageName, this.Id, val1, val2, val3);
            }
            else
            {
                BridgeConnector.Socket.Emit(messageName, val1, val2, val3);
            }
        }
    }
}
