using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nito.AsyncEx;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

namespace ElectronNET.API
{
    internal static class BridgeConnector
    {
        internal static class EventTasks<T>
        {
            //Although SocketIO already manage event handlers, we need to manage this here as well for the OnResult calls,
            //because SocketIO will simply replace the existing event handler on every call to On(key, ...) , which means there is 
            //a race condition between On / Off calls that can lead to tasks deadlocking forever without ever triggering their On handler

            private static readonly Dictionary<string, TaskCompletionSource<T>> _taskCompletionSources = new();
            private static readonly Dictionary<string, string> _eventKeys = new();
            private static readonly object _lock = new();

            /// <summary>
            /// Get or add a new TaskCompletionSource<typeparamref name="T"/> for a given event key
            /// </summary>
            /// <param name="key"></param>
            /// <param name="eventKey"></param>
            /// <param name="taskCompletionSource"></param>
            /// <param name="waitThisFirstAndThenTryAgain"></param>
            /// <returns>Returns true if a new TaskCompletionSource<typeparamref name="T"/> was added to the dictionary</returns>
            internal static bool TryGetOrAdd(string key, string eventKey, out TaskCompletionSource<T> taskCompletionSource, out Task waitThisFirstAndThenTryAgain)
            {
                lock (_lock)
                {
                    if (!_taskCompletionSources.TryGetValue(key, out taskCompletionSource))
                    {
                        taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
                        _taskCompletionSources[key] = taskCompletionSource;
                        _eventKeys[key] = eventKey;
                        waitThisFirstAndThenTryAgain = null;
                        return true; //Was added, so we need to also register the socket events
                    }

                    if (_eventKeys.TryGetValue(key, out var existingEventKey) && existingEventKey == eventKey)
                    {
                        waitThisFirstAndThenTryAgain = null;
                        return false; //No need to register the socket events twice
                    }

                    waitThisFirstAndThenTryAgain = taskCompletionSource.Task; //Will need to try again after the previous existing one is done

                    taskCompletionSource = null;

                    return true; //Need to register the socket events, but must first await the previous task to complete
                }
            }

            /// <summary>
            /// Clean up the TaskCompletionSource<typeparamref name="T"/> from the dictionary if and only if it is the same as the passed argument
            /// </summary>
            /// <param name="key"></param>
            /// <param name="eventKey"></param>
            /// <param name="taskCompletionSource"></param>
            internal static void DoneWith(string key, string eventKey, TaskCompletionSource<T> taskCompletionSource)
            {
                lock (_lock)
                {
                    if (_taskCompletionSources.TryGetValue(key, out var existingTaskCompletionSource)
                        && ReferenceEquals(existingTaskCompletionSource, taskCompletionSource))
                    {
                        _taskCompletionSources.Remove(key);
                    }

                    if (_eventKeys.TryGetValue(key, out var existingEventKey) && existingEventKey == eventKey)
                    {
                        _eventKeys.Remove(key);
                    }
                }
            }
        }

        private static SocketIO _socket;

        private static readonly object _syncRoot = new();

        private static readonly SemaphoreSlim _socketSemaphoreEmit = new(1, 1);
        private static readonly SemaphoreSlim _socketSemaphoreHandlers = new(1, 1);

        private static AsyncManualResetEvent _connectedSocketEvent = new AsyncManualResetEvent();

        private static Dictionary<string, Action<SocketIOResponse>> _eventHandlers = new();

        private static Task<SocketIO> _waitForConnection
        {
            get
            {
                EnsureSocketTaskIsCreated();
                return GetSocket();
            }
        }

        private static async Task<SocketIO> GetSocket()
        {
            await _connectedSocketEvent.WaitAsync();
            return _socket;
        }

        public static bool IsConnected => _waitForConnection is Task task && task.IsCompletedSuccessfully;

        public static void Emit(string eventString, params object[] args)
        {
            //We don't care about waiting for the event to be emitted, so this doesn't need to be async 

            Task.Run(() => EmitAsync(eventString, args));
        }

        private static async Task EmitAsync(string eventString, object[] args)
        {
            if (App.SocketDebug)
            {
                Log("Sending event {0}", eventString);
            }

            var socket = await _waitForConnection;

            await _socketSemaphoreEmit.WaitAsync();

            try
            {
                await socket.EmitAsync(eventString, args);
            }
            finally
            {
                _socketSemaphoreEmit.Release();
            }

            if (App.SocketDebug)
            {
                Log($"Sent event {eventString}");
            }
        }

        /// <summary>
        /// This method is only used on places where we need to be sure the event was sent on the socket, such as Quit, Exit, Relaunch and QuitAndInstall methods
        /// </summary>
        /// <param name="eventString"></param>
        /// <param name="args"></param>
        internal static void EmitSync(string eventString, params object[] args)
        {
            if (App.SocketDebug)
            {
                Log("Sending event {0}", eventString);
            }

            Task.Run(async () =>
            {
                var socket = await _waitForConnection;
                try
                {
                    await _socketSemaphoreEmit.WaitAsync();
                    await socket.EmitAsync(eventString, args);
                }
                finally
                {
                    _socketSemaphoreEmit.Release();
                }
            }).Wait();


            if (App.SocketDebug)
            {
                Log("Sent event {0}", eventString);
            }
        }

        public static void Off(string eventString)
        {
            EnsureSocketTaskIsCreated();

            _socketSemaphoreHandlers.Wait();
            try
            {
                if (_eventHandlers.ContainsKey(eventString))
                {
                    _eventHandlers.Remove(eventString);
                }

                _socket.Off(eventString);
            }
            finally
            {
                _socketSemaphoreHandlers.Release();
            }
        }

        public static void On(string eventString, Action fn)
        {
            EnsureSocketTaskIsCreated();

            _socketSemaphoreHandlers.Wait();
            try
            {
                if (_eventHandlers.ContainsKey(eventString))
                {
                    _eventHandlers.Remove(eventString);
                }

                _eventHandlers.Add(eventString, _ =>
                {
                    try
                    {
                        fn();
                    }
                    catch (Exception E)
                    {
                        LogError(E, "Error running handler for event {0}", eventString);
                    }
                });

                _socket.On(eventString, _eventHandlers[eventString]);
            }
            finally
            {
                _socketSemaphoreHandlers.Release();
            }
        }

        public static void On<T>(string eventString, Action<T> fn)
        {
            EnsureSocketTaskIsCreated();

            _socketSemaphoreHandlers.Wait();
            try
            {
                if (_eventHandlers.ContainsKey(eventString))
                {
                    _eventHandlers.Remove(eventString);
                }

                _eventHandlers.Add(eventString, o =>
                {
                    try
                    {
                        fn(o.GetValue<T>(0));
                    }
                    catch (Exception E)
                    {
                        LogError(E, "Error running handler for event {0}", eventString);
                    }
                });

                _socket.On(eventString, _eventHandlers[eventString]);
            }
            finally
            {
                _socketSemaphoreHandlers.Release();
            }
        }

        private static void RehookHandlers(SocketIO newSocket)
        {
            _socketSemaphoreHandlers.Wait();
            try
            {
                foreach (var kv in _eventHandlers)
                {
                    newSocket.On(kv.Key, kv.Value);
                }
            }
            finally
            {
                _socketSemaphoreHandlers.Release();
            }
        }

        public static void Once<T>(string eventString, Action<T> fn)
        {
            On<T>(eventString, (o) =>
            {
                Off(eventString);
                fn(o);
            });
        }

        public static async Task<T> OnResult<T>(string triggerEvent, string completedEvent, params object[] args)
        {
            string eventKey = completedEvent;

            if (args is object && args.Length > 0) // If there are arguments passed, we generate a unique event key with the arguments
                                                   // this allow us to wait for previous events first before registering new ones
            {
                var hash = new HashCode();
                foreach (var obj in args)
                {
                    hash.Add(obj);
                }
                eventKey = $"{eventKey}-{(uint)hash.ToHashCode()}";
            }

            if (EventTasks<T>.TryGetOrAdd(completedEvent, eventKey, out var taskCompletionSource, out var waitThisFirstAndThenTryAgain))
            {
                if (waitThisFirstAndThenTryAgain is object)
                {
                    //There was a pending call with different parameters, so we need to wait that first and then call here again
                    try
                    {
                        await waitThisFirstAndThenTryAgain;
                    }
                    catch
                    {
                        //Ignore any exceptions here so we can set a new event below
                        //The exception will also be visible to the original first caller due to taskCompletionSource.Task
                    }

                    //Try again to set the event
                    return await OnResult<T>(triggerEvent, completedEvent, args);
                }
                else
                {
                    //A new TaskCompletionSource was added, so we need to register the completed event here

                    On<T>(completedEvent, (result) =>
                    {
                        Off(completedEvent);
                        taskCompletionSource.SetResult(result);
                        EventTasks<T>.DoneWith(completedEvent, eventKey, taskCompletionSource);
                    });

                    await EmitAsync(triggerEvent, args);
                }
            }

            return await taskCompletionSource.Task;
        }


        public static async Task<T> OnResult<T>(string triggerEvent, string completedEvent, CancellationToken cancellationToken, params object[] args)
        {
            string eventKey = completedEvent;

            if (args is object && args.Length > 0) // If there are arguments passed, we generate a unique event key with the arguments
                                                   // this allow us to wait for previous events first before registering new ones
            {
                var hash = new HashCode();
                foreach (var obj in args)
                {
                    hash.Add(obj);
                }
                eventKey = $"{eventKey}-{(uint)hash.ToHashCode()}";
            }

            if (EventTasks<T>.TryGetOrAdd(completedEvent, eventKey, out var taskCompletionSource, out var waitThisFirstAndThenTryAgain))
            {
                if (waitThisFirstAndThenTryAgain is object)
                {
                    //There was a pending call with different parameters, so we need to wait that first and then call here again
                    try
                    {
                        await Task.Run(() => waitThisFirstAndThenTryAgain, cancellationToken);
                    }
                    catch
                    {
                        //Ignore any exceptions here so we can set a new event below
                        //The exception will also be visible to the original first caller due to taskCompletionSource.Task
                    }

                    //Try again to set the event
                    return await OnResult<T>(triggerEvent, completedEvent, cancellationToken, args);
                }
                else
                {
                    using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
                    {
                        //A new TaskCompletionSource was added, so we need to register the completed event here

                        On<T>(completedEvent, (result) =>
                        {
                            Off(completedEvent);
                            taskCompletionSource.SetResult(result);
                            EventTasks<T>.DoneWith(completedEvent, eventKey, taskCompletionSource);
                        });

                        Emit(triggerEvent, args);
                    }
                }
            }

            return await taskCompletionSource.Task;
        }

        internal static void Log(string formatString, params object[] args)
        {
            if (Logger is object)
            {
                Logger.LogInformation(formatString, args);
            }
            else
            {
                Console.WriteLine(formatString, args);
            }
        }

        internal static void LogError(Exception E, string formatString, params object[] args)
        {
            if (Logger is object)
            {
                Logger.LogError(E, formatString, args);
            }
            else
            {
                Console.WriteLine(formatString, args);
                Console.WriteLine(E.ToString());
            }
        }

        private static Thread _backgroundMonitorThread;

        private static void EnsureSocketTaskIsCreated()
        {
            if (_socket is null)
            {
                if (string.IsNullOrWhiteSpace(AuthKey))
                {
                    throw new Exception("You must call Electron.ReadAuth() first thing on your main entry point.");
                }

                if (HybridSupport.IsElectronActive)
                {
                    lock (_syncRoot)
                    {
                        if (_socket is null)
                        {
                            if (HybridSupport.IsElectronActive)
                            {
                                var socket = new SocketIO($"http://localhost:{BridgeSettings.SocketPort}", new SocketIOOptions()
                                {
                                    EIO = 4,
                                    Reconnection = true,
                                    ReconnectionAttempts = int.MaxValue,
                                    ReconnectionDelay = 500,
                                    ReconnectionDelayMax = 2000,
                                    RandomizationFactor = 0.5,
                                    ConnectionTimeout = TimeSpan.FromSeconds(10),
                                    Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
                                });

                                socket.JsonSerializer = new CamelCaseNewtonsoftJsonSerializer();

                                _connectedSocketEvent.Reset();

                                socket.OnConnected += (_, __) =>
                                {
                                    Task.Run(async () =>
                                    {
                                        await socket.EmitAsync("auth", AuthKey);
                                        _connectedSocketEvent.Set();
                                        Log("ElectronNET socket {1} connected on port {0}!", BridgeSettings.SocketPort, socket.Id);
                                    });
                                };

                                socket.OnReconnectAttempt += (_, __) =>
                                {
                                    _connectedSocketEvent.Reset();
                                    Log("ElectronNET socket {1} is trying to reconnect on port {0}...", BridgeSettings.SocketPort, socket.Id);
                                };

                                socket.OnReconnectError += (_, ex) =>
                                {
                                    _connectedSocketEvent.Reset();
                                    Log("ElectronNET socket {1} failed to connect {0}", ex, socket.Id);
                                };


                                socket.OnReconnectFailed += (_, ex) =>
                                {
                                    _connectedSocketEvent.Reset();
                                    Log("ElectronNET socket {1} failed to reconnect {0}", ex, socket.Id);
                                };

                                socket.OnReconnected += (_, __) =>
                                {
                                    _connectedSocketEvent.Set();
                                    Log("ElectronNET socket {1} reconnected on port {0}...", BridgeSettings.SocketPort, socket.Id);
                                };

                                socket.OnDisconnected += (_, reason) =>
                                {
                                    _connectedSocketEvent.Reset();
                                    Log("ElectronNET socket {2} disconnected with reason {0}, trying to reconnect on port {1}!", reason, BridgeSettings.SocketPort, socket.Id);
                                };

                                socket.OnError += (_, msg) =>
                                {
                                    //_connectedSocketEvent.Reset();
                                    Log("ElectronNET socket {1} error: {0}...", msg, socket.Id);
                                };

                                _socket = socket;

                                Task.Run(async () =>
                                {
                                    try
                                    {
                                        await socket.ConnectAsync();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());

                                        if (!App.TryRaiseOnSocketConnectFail())
                                        {
                                            Environment.Exit(0xDEAD);
                                        }
                                    }
                                });

                                RehookHandlers(socket);
                            }
                            else
                            {
                                throw new Exception("Missing Socket Port");
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Missing Socket Port");
                }
            }
        }

        internal static ILogger<App> Logger { private get; set; }
        internal static string AuthKey { get; set; } = null;

        private class CamelCaseNewtonsoftJsonSerializer : NewtonsoftJsonSerializer
        {
            public CamelCaseNewtonsoftJsonSerializer() : base()
            {
                OptionsProvider = () => new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                };
            }
        }
    }
}