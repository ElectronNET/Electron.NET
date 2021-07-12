using System;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

namespace ElectronNET.API
{
    internal static class BridgeConnector
    {
        private static SocketIO _socket;
        private static object _syncRoot = new object();


        public static void Emit(string eventString, params object[] args) => Socket.EmitAsync(eventString, args);
        public static void Off(string eventString)                        => Socket.Off(eventString);
        public static void On(string eventString, Action fn)              => Socket.On(eventString, _ => fn());
        public static void On<T>(string eventString, Action<T> fn) => Socket.On(eventString, (o) =>
                                                                      {
                                                                          fn(o.GetValue<T>(0));
                                                                      });

        public static void Once<T>(string eventString, Action<T> fn)    => Socket.On(eventString, (o) =>
        {
            Socket.Off(eventString);
            fn(o.GetValue<T>(0));
        });


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
                                var socket = new SocketIO($"http://localhost:{BridgeSettings.SocketPort}", new SocketIOOptions()
                                {
                                    EIO = 3
                                });

                                socket.JsonSerializer = new NewtonsoftJsonSerializer(socket.Options.EIO);


                                socket.OnConnected += (_, __) =>
                                {
                                    Console.WriteLine("BridgeConnector connected!");
                                };

                                socket.ConnectAsync().Wait();

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
    }

    public interface IListener : System.IComparable<IListener>
    {
        int GetId();
        void Call(params object[] args);
    }

    public class ListenerImpl : IListener
    {
        private static int id_counter = 0;
        private int Id;
        private readonly Action fn1;
        private readonly Action<object> fn;

        public ListenerImpl(Action<object> fn)
        {

            this.fn = fn;
            this.Id = id_counter++;
        }

        public ListenerImpl(Action fn)
        {

            this.fn1 = fn;
            this.Id = id_counter++;
        }

        public void Call(params object[] args)
        {
            if (fn != null)
            {
                var arg = args.Length > 0 ? args[0] : null;
                fn(arg);
            }
            else
            {
                fn1();
            }
        }

        public int CompareTo(IListener other)
        {
            return this.GetId().CompareTo(other.GetId());
        }

        public int GetId()
        {
            return Id;
        }
    }
}
