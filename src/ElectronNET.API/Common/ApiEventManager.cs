using System;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;

namespace ElectronNET.Common;

internal static class ApiEventManager
{
    internal static void AddEvent(string eventName, object id, Action callback, Action value, string suffix = "")
    {
        if (callback == null)
        {
            BridgeConnector.Socket.On(eventName + id, () => { callback(); });
            BridgeConnector.Socket.Emit($"register-{eventName}{suffix}", id);
        }

        callback += value;
    }

    internal static void RemoveEvent(string eventName, object id, Action callback, Action value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Socket.Off(eventName + id);
    }

    internal static void AddEvent<T>(string eventName, object id, Action<T> callback, Action<T> value, Func<object, T> converter, string suffix = "")
    {
        if (callback == null)
        {
            BridgeConnector.Socket.On(eventName + id, (args) =>
            {
                var converted = converter.Invoke(args);
                callback(converted);
            });
            BridgeConnector.Socket.Emit($"register-{eventName}{suffix}", id);
        }

        callback += value;
    }

    internal static void AddEvent<T>(string eventName, object id, Action<T> callback, Action<T> value)
    {
        if (callback == null)
        {
            BridgeConnector.Socket.On<T>(eventName + id, (args) => callback(args));
            BridgeConnector.Socket.Emit($"register-{eventName}", id);
        }

        callback += value;
    }

    internal static void RemoveEvent<T>(string eventName, object id, Action<T> callback, Action<T> value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Socket.Off(eventName + id);
    }

    internal static void AddTrayEvent(string eventName, object id, Action<TrayClickEventArgs, Rectangle> callback, Action<TrayClickEventArgs, Rectangle> value)
    {
        if (callback == null)
        {
            BridgeConnector.Socket.On<dynamic>(eventName + id, (result) =>
            {
                var args = ((JArray)result).ToObject<object[]>();
                var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
                var bounds = ((JObject)args[1]).ToObject<Rectangle>();
                callback(trayClickEventArgs, bounds);
            });
            BridgeConnector.Socket.Emit($"register-{eventName}", id);
            callback += value;
        }
    }

    internal static void RemoveTrayEvent(string eventName, object id, Action<TrayClickEventArgs, Rectangle> callback, Action<TrayClickEventArgs, Rectangle> value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Socket.Off(eventName + id);
    }

    internal static void AddScreenEvent(string eventName, object id, Action<Display, string[]> callback, Action<Display, string[]> value)
    {
        if (callback == null)
        {
            BridgeConnector.Socket.On(eventName + id, (args) =>
            {
                var display = ((JArray)args).First.ToObject<Display>();
                var metrics = ((JArray)args).Last.ToObject<string[]>();
                callback(display, metrics);
            });
            BridgeConnector.Socket.Emit($"register-{eventName}", id);
            callback += value;
        }
    }

    internal static void RemoveScreenEvent(string eventName, object id, Action<Display, string[]> callback, Action<Display, string[]> value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Socket.Off(eventName + id);
    }
}