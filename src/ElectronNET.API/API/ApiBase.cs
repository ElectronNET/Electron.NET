namespace ElectronNET.API
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using ElectronNET.Common;

    public abstract class ApiBase
    {
        internal const int PropertyTimeout = 1000;

        private readonly string objectName;
        private readonly ConcurrentDictionary<string, PropertyGetter> propertyGetters = new ConcurrentDictionary<string, PropertyGetter>();
        private readonly ConcurrentDictionary<string, string> propertyEventNames = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> propertyMessageNames = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> methodMessageNames = new ConcurrentDictionary<string, string>();
        private readonly object objLock = new object();

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

        protected Task<T> GetPropertyAsync<T>([CallerMemberName] string callerName = null)
        {
            Debug.Assert(callerName != null, nameof(callerName) + " != null");

            lock (this.objLock)
            {
                return this.propertyGetters.GetOrAdd(callerName, _ =>
                {
                    var getter = new PropertyGetter<T>(this, callerName, PropertyTimeout);

                    getter.Task<T>().ContinueWith(_ =>
                    {
                        lock (this.objLock)
                        {
                            return this.propertyGetters.TryRemove(callerName, out var _);
                        }
                    });

                    return getter;
                }).Task<T>();
            }
        }

        internal abstract class PropertyGetter
        {
            public abstract Task<T> Task<T>();
        }

        internal class PropertyGetter<T> : PropertyGetter
        {
            private readonly Task<T> tcsTask;
            private TaskCompletionSource<T> tcs;

            public PropertyGetter(ApiBase apiBase, string callerName, int timeoutMs)
            {
                this.tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
                this.tcsTask = this.tcs.Task;

                var eventName = apiBase.propertyEventNames.GetOrAdd(callerName, s => $"{apiBase.objectName}-{s.StripAsync().LowerFirst()}{apiBase.SocketEventCompleteSuffix}");
                var messageName = apiBase.propertyMessageNames.GetOrAdd(callerName, s => apiBase.objectName + s.StripAsync());

                BridgeConnector.Socket.On<T>(eventName, (result) =>
                {
                    BridgeConnector.Socket.Off(eventName);

                    lock (this)
                    {
                        this.tcs?.SetResult(result);
                        this.tcs = null;
                    }
                });

                if (apiBase.Id >= 0)
                {
                    BridgeConnector.Socket.Emit(messageName, apiBase.Id);
                }
                else
                {
                    BridgeConnector.Socket.Emit(messageName);
                }

                System.Threading.Tasks.Task.Delay(ApiBase.PropertyTimeout).ContinueWith(_ =>
                {
                    if (this.tcs != null)
                    {
                        lock (this)
                        {
                            if (this.tcs != null)
                            {
                                var ex = new TimeoutException($"No response after {timeoutMs:D}ms trying to retrieve value {apiBase.objectName}.{callerName}()");
                                this.tcs.TrySetException(ex);
                                this.tcs = null;
                            }
                        }
                    }
                });
            }

            public override Task<T1> Task<T1>()
            {
                return this.tcsTask as Task<T1>;
            }
        }
    }
}
