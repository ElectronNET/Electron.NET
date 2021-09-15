using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SocketIOClient;
using SocketIOClient.JsonSerializer;
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

                    if(_eventKeys.TryGetValue(key, out var existingEventKey) && existingEventKey == eventKey)
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
        
        private static TaskCompletionSource<SocketIO> _connectedSocketTask = new();
        
        private static Task<SocketIO> _waitForConnection
        {
            get
            {
                EnsureSocketTaskIsCreated();
                return _connectedSocketTask.Task;
            }
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
                _socket.On(eventString, _ =>
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
                _socket.On(eventString, (o) =>
                {
                    try
                    {
                        fn(o.GetValue<T>(0));
                    }
                    catch(Exception E)
                    {
                        LogError(E, "Error running handler for event {0}", eventString);
                    }
                });
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
                foreach(var obj in args)
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

        private static void EnsureSocketTaskIsCreated()
        {
            if (_socket is null)
            {
                if (HybridSupport.IsElectronActive)
                {
                    lock (_syncRoot)
                    {
                        if (_socket is null && HybridSupport.IsElectronActive)
                        {
                            var socket = new SocketIO($"http://localhost:{BridgeSettings.SocketPort}", new SocketIOOptions()
                            {
                                EIO = 3,
                                Reconnection = true,
                                ReconnectionAttempts = int.MaxValue,
                                ReconnectionDelay = 1000,
                                ReconnectionDelayMax = 5000,
                                RandomizationFactor = 0.5,
                                ConnectionTimeout = TimeSpan.FromSeconds(10)
                            });

                            socket.JsonSerializer = new CamelCaseNewtonsoftJsonSerializer(socket.Options.EIO);

                            socket.OnConnected += (_, __) =>
                            {
                                _connectedSocketTask.TrySetResult(socket);
                                Log("ElectronNET socket connected on port {0}!", BridgeSettings.SocketPort);
                            };

                            socket.OnReconnectAttempt += (_, __) =>
                            {
                                _connectedSocketTask = new();
                                Log("ElectronNET socket is trying to reconnect on port {0}...", BridgeSettings.SocketPort);
                            };

                            socket.OnReconnectError += (_, ex) =>
                            {
                                Log("ElectronNET socket failed to connect {0}", ex);
                            };

                            socket.OnReconnected += (_, __) =>
                            {
                                _connectedSocketTask.TrySetResult(socket);
                                Log("ElectronNET socket reconnected on port {0}...", BridgeSettings.SocketPort);
                            };


                            socket.OnDisconnected += async (_, reason) =>
                            {
                                _connectedSocketTask = new();

                                Log("ElectronNET socket disconnected with reason {0}, trying to reconnect on port {1}!", reason, BridgeSettings.SocketPort);

                                int i = 0;
                                    
                                double miliseconds = 500;

                                while (true)
                                {
                                    try
                                    {
                                        if (!socket.Connected)
                                        {
                                            await socket.ConnectAsync();
                                            _connectedSocketTask.TrySetResult(socket); //Probably was already on the OnConnected call
                                        }
                                        return;
                                    }
                                    catch (Exception e)
                                    {
                                        LogError(e, "Failed to reconnect, will try again in {0} ms.", miliseconds * 2);
                                    }

                                    await Task.Delay(TimeSpan.FromMilliseconds(miliseconds));

                                    miliseconds = Math.Min(60_000, Math.Pow(2, i) + 500);

                                    i++;
                                }
                            };

                            Task.Run(async () =>
                            {
                                await socket.ConnectAsync();
                                _connectedSocketTask.TrySetResult(socket); //Probably was already on the OnConnected call
                            });

                            _socket = socket;
                        }
                        else
                        {
                            throw new Exception("Missing Socket Port");
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

        private class CamelCaseNewtonsoftJsonSerializer : NewtonsoftJsonSerializer
        {
            public CamelCaseNewtonsoftJsonSerializer(int eio) : base(eio)
            {
            }

            public override JsonSerializerSettings CreateOptions()
            {
                return new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                };
            }
        }
    }
}
