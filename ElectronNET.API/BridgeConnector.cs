using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private static object _syncRoot = new object();

        public static void Emit(string eventString, params object[] args)
        {
            //We don't care about waiting for the event to be emitted, so this doesn't need to be async 

            Task.Run(async () =>
            {
                if (App.SocketDebug)
                {
                    Console.WriteLine($"Sending event {eventString}");
                }

                await Socket.EmitAsync(eventString, args);

                if (App.SocketDebug)
                {
                    Console.WriteLine($"Sent event {eventString}");
                }
            });
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
                Console.WriteLine($"Sending event {eventString}");
            }

            Socket.EmitAsync(eventString, args).Wait();

            if (App.SocketDebug)
            {
                Console.WriteLine($"Sent event {eventString}");
            }
        }

        public static void Off(string eventString)
        {
            Socket.Off(eventString);
        }

        public static void On(string eventString, Action fn)
        {
            Socket.On(eventString, _ => fn());
        }

        public static void On<T>(string eventString, Action<T> fn)
        {
            Socket.On(eventString, (o) => fn(o.GetValue<T>(0)));
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

                    Emit(triggerEvent, args);
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
        private static SocketIO Socket
        {
            get
            {
                if (_socket is null)
                {
                    if (HybridSupport.IsElectronActive)
                    {

                        lock (_syncRoot)
                        {
                            if (_socket is null && HybridSupport.IsElectronActive)
                            {
                                var socket = new SocketIO($"http://localhost:{BridgeSettings.SocketPort}");

                                socket.JsonSerializer = new CamelCaseNewtonsoftJsonSerializer(socket.Options.EIO);


                                socket.OnConnected += (_, __) =>
                                {
                                    Console.WriteLine("BridgeConnector connected!");
                                };

                                socket.ConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                                _socket = socket;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Missing Socket Port");
                    }
                }

                return _socket;
            }
        }

        internal static SocketIO GetSocket() => Socket;

        internal static async Task StartSocket()
        {
            await Task.Run(GetSocket);
        }

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
                    DefaultValueHandling = DefaultValueHandling.Ignore
                };
            }
        }
        public static async Task<T> GetValueOverSocketAsync<T>(string eventString, string eventCompletedString)
        {
            CancellationToken cancellationToken = new();
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<T>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On(eventCompletedString, (value) =>
                {
                    BridgeConnector.Socket.Off(eventCompletedString);

                    if (value == null)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') returned null. Socket loop hang.");
                        taskCompletionSource.SetCanceled();
                        return;
                    }

                    try
                    {
                        taskCompletionSource.SetResult( new JValue(value).ToObject<T>() );
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') exception: {e.Message}. Socket loop hung.");
                    }
                });

                await BridgeConnector.Socket.EmitAsync(eventString);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }

        public static async Task<T> GetObjectOverSocketAsync<T>(string eventString, string eventCompletedString)
        {
            CancellationToken cancellationToken = new();
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<T>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On(eventCompletedString, (value) =>
                {
                    BridgeConnector.Socket.Off(eventCompletedString);

                    if (value == null)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') returned null. Socket loop hang.");
                        taskCompletionSource.SetCanceled();
                        return;
                    }

                    try
                    {
                        var json = value.GetValue().GetRawText();
                        taskCompletionSource.SetResult(new JObject(json).ToObject<T>());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') exception: {e.Message}. Socket loop hung.");
                    }
                });

                await BridgeConnector.Socket.EmitAsync(eventString);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }

        public static async Task<T> GetArrayOverSocketAsync<T>(string eventString, string eventCompletedString)
        {
            CancellationToken cancellationToken = new();
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<T>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On(eventCompletedString, (value) =>
                {
                    BridgeConnector.Socket.Off(eventCompletedString);
                    if (value == null)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') returned null. Socket loop hang.");
                        taskCompletionSource.SetCanceled();
                        return;
                    }

                    try
                    {
                        var json = value.GetValue().GetRawText();
                        taskCompletionSource.SetResult(new JArray(json).ToObject<T>());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: BridgeConnector (event: '{eventString}') exception: {e.Message}. Socket loop hung.");
                    }
                });

                await BridgeConnector.Socket.EmitAsync(eventString);

                return await taskCompletionSource.Task.ConfigureAwait(false);
            }
        }
    }
}
