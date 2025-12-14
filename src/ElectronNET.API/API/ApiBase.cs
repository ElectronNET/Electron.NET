// ReSharper disable InconsistentNaming

namespace ElectronNET.API
{
    using Common;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public abstract class ApiBase
    {
        protected enum SocketTaskEventNameTypes
        {
            DashesLowerFirst,
            NoDashUpperFirst
        }

        protected enum SocketTaskMessageNameTypes
        {
            DashesLowerFirst,
            NoDashUpperFirst
        }

        protected enum SocketEventNameTypes
        {
            DashedLower,
            CamelCase,
        }

        private static readonly TimeSpan InvocationTimeout = 1000.ms();

        private readonly string objectName;
        private readonly ConcurrentDictionary<string, Invocator> invocators;
        private readonly ConcurrentDictionary<string, string> invocationEventNames = new();
        private readonly ConcurrentDictionary<string, string> invocationMessageNames = new();
        private readonly ConcurrentDictionary<string, string> methodMessageNames = new();
        private static readonly ConcurrentDictionary<string, EventContainer> eventContainers = new();
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Invocator>> AllInvocators = new();

        private readonly object objLock = new object();

        public virtual int Id
        {
            get => -1;

            // ReSharper disable once ValueParameterNotUsed
            protected set
            {
            }
        }

        protected abstract SocketTaskEventNameTypes SocketTaskEventNameType { get; }
        protected virtual SocketTaskMessageNameTypes SocketTaskMessageNameType => SocketTaskMessageNameTypes.NoDashUpperFirst;
        protected virtual SocketEventNameTypes SocketEventNameType => SocketEventNameTypes.DashedLower;

        protected ApiBase()
        {
            this.objectName = this.GetType().Name.LowerFirst();
            this.invocators = AllInvocators.GetOrAdd(this.objectName, _ => new ConcurrentDictionary<string, Invocator>());
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

        protected Task<T> InvokeAsync<T>(object arg = null, [CallerMemberName] string callerName = null)
        {
            return this.InvokeAsyncWithTimeout<T>(InvocationTimeout, arg, callerName);
        }

        protected Task<T> InvokeAsyncWithTimeout<T>(TimeSpan invocationTimeout, object arg = null, [CallerMemberName] string callerName = null)
        {
            Debug.Assert(callerName != null, nameof(callerName) + " != null");

            lock (this.objLock)
            {
                return this.invocators.GetOrAdd(callerName, _ =>
                {
                    var getter = new Invocator<T>(this, callerName, invocationTimeout, arg);

                    getter.Task<T>().ContinueWith(_ =>
                    {
                        lock (this.objLock)
                        {
                            return this.invocators.TryRemove(callerName, out var _);
                        }
                    });

                    return getter;
                }).Task<T>();
            }
        }

        protected void AddEvent(Action value, int? id = null, [CallerMemberName] string callerName = null)
        {
            Debug.Assert(callerName != null, nameof(callerName) + " != null");
            var eventName = this.EventName(callerName);

            var eventKey = this.EventKey(eventName, id);

            lock (this.objLock)
            {
                var container = eventContainers.GetOrAdd(eventKey, _ =>
                {
                    var container = new EventContainer();
                    BridgeConnector.Socket.On(eventKey, container.OnEventAction);
                    BridgeConnector.Socket.Emit($"register-{eventName}", id);
                    return container;
                });

                container.Register(value);
            }
        }

        protected void RemoveEvent(Action value, int? id = null, [CallerMemberName] string callerName = null)
        {
            Debug.Assert(callerName != null, nameof(callerName) + " != null");
            var eventName = this.EventName(callerName);
            var eventKey = this.EventKey(eventName, id);

            lock (this.objLock)
            {
                if (eventContainers.TryGetValue(eventKey, out var container) && !container.Unregister(value))
                {
                    BridgeConnector.Socket.Off(eventKey);
                    eventContainers.TryRemove(eventKey, out _);
                }
            }
        }

        protected void AddEvent<T>(Action<T> value, int? id = null, [CallerMemberName] string callerName = null)
        {
            Debug.Assert(callerName != null, nameof(callerName) + " != null");

            var eventName = this.EventName(callerName);
            var eventKey = this.EventKey(eventName, id);

            lock (this.objLock)
            {
                var container = eventContainers.GetOrAdd(eventKey, _ =>
                {
                    var container = new EventContainer();
                    BridgeConnector.Socket.On<T>(eventKey, container.OnEventActionT);
                    BridgeConnector.Socket.Emit($"register-{eventName}", id);
                    return container;
                });

                container.Register(value);
            }
        }

        protected void RemoveEvent<T>(Action<T> value, int? id = null, [CallerMemberName] string callerName = null)
        {
            Debug.Assert(callerName != null, nameof(callerName) + " != null");
            var eventName = this.EventName(callerName);
            var eventKey = this.EventKey(eventName, id);

            lock (this.objLock)
            {
                if (eventContainers.TryGetValue(eventKey, out var container) && !container.Unregister(value))
                {
                    BridgeConnector.Socket.Off(eventKey);
                    eventContainers.TryRemove(eventKey, out _);
                }
            }
        }

        private string EventName(string callerName)
        {
            switch (this.SocketEventNameType)
            {
                case SocketEventNameTypes.DashedLower:
                    return $"{this.objectName}-{callerName.ToDashedEventName()}";
                case SocketEventNameTypes.CamelCase:
                    return $"{this.objectName}-{callerName.ToCamelCaseEventName()}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string EventKey(string eventName, int? id)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}{1:D}", eventName, id);
        }

        internal abstract class Invocator
        {
            public abstract Task<T> Task<T>();
        }

        internal class Invocator<T> : Invocator
        {
            private readonly Task<T> tcsTask;
            private TaskCompletionSource<T> tcs;

            public Invocator(ApiBase apiBase, string callerName, TimeSpan timeout, object arg = null)
            {
                this.tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
                this.tcsTask = this.tcs.Task;

                string eventName;
                string messageName;

                switch (apiBase.SocketTaskEventNameType)
                {
                    case SocketTaskEventNameTypes.DashesLowerFirst:
                        eventName = apiBase.invocationEventNames.GetOrAdd(callerName, s => $"{apiBase.objectName}-{s.StripAsync().LowerFirst()}-completed");
                        break;
                    case SocketTaskEventNameTypes.NoDashUpperFirst:
                        eventName = apiBase.invocationEventNames.GetOrAdd(callerName, s => $"{apiBase.objectName}{s.StripAsync()}Completed");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (apiBase.SocketTaskMessageNameType)
                {
                    case SocketTaskMessageNameTypes.DashesLowerFirst:
                        messageName = apiBase.invocationMessageNames.GetOrAdd(callerName, s => $"{apiBase.objectName}-{s.StripAsync().LowerFirst()}");
                        break;
                    case SocketTaskMessageNameTypes.NoDashUpperFirst:
                        messageName = apiBase.invocationMessageNames.GetOrAdd(callerName, s => apiBase.objectName + s.StripAsync());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                BridgeConnector.Socket.Once<T>(eventName, (result) =>
                {
                    lock (this)
                    {
                        try
                        {
                            var value = result;
                            this.tcs?.SetResult(value);
                        }
                        catch (Exception ex)
                        {
                            this.tcs?.TrySetException(ex);
                        }
                        finally
                        {
                            this.tcs = null;
                        }
                    }
                });

                if (arg != null)
                {
                    _ = apiBase.Id >= 0 ? BridgeConnector.Socket.Emit(messageName, apiBase.Id, arg) : BridgeConnector.Socket.Emit(messageName, arg);
                }
                else
                {
                    _ = apiBase.Id >= 0 ? BridgeConnector.Socket.Emit(messageName, apiBase.Id) : BridgeConnector.Socket.Emit(messageName);
                }

                System.Threading.Tasks.Task.Delay(timeout).ContinueWith(_ =>
                {
                    if (this.tcs != null)
                    {
                        lock (this)
                        {
                            if (this.tcs != null)
                            {
                                var ex = new TimeoutException($"No response after {timeout:D}ms trying to retrieve value {apiBase.objectName}.{callerName}()");
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

        [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
        private class EventContainer
        {
            private Action eventAction;
            private Delegate eventActionT;

            private Action<T> GetEventActionT<T>()
            {
                return (Action<T>)this.eventActionT;
            }

            private void SetEventActionT<T>(Action<T> actionT)
            {
                this.eventActionT = actionT;
            }

            public void OnEventAction() => this.eventAction?.Invoke();

            public void OnEventActionT<T>(T p) => this.GetEventActionT<T>()?.Invoke(p);

            public void Register(Action receiver)
            {
                this.eventAction += receiver;
            }

            public void Register<T>(Action<T> receiver)
            {
                var actionT = this.GetEventActionT<T>();
                actionT += receiver;
                this.SetEventActionT(actionT);
            }

            public bool Unregister(Action receiver)
            {
                this.eventAction -= receiver;
                return this.eventAction != null;
            }

            public bool Unregister<T>(Action<T> receiver)
            {
                var actionT = this.GetEventActionT<T>();
                actionT -= receiver;
                this.SetEventActionT(actionT);

                return actionT != null;
            }
        }
    }
}