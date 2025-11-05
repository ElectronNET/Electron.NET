using System;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;

namespace ElectronNET.Common;

internal class ApiEventManager
{
    internal T Deserialize<T>(Func<T> action)
    {
        return action.Invoke();
    }
    
    internal static void AddEvent(string eventName, object id, Action callback, Action value, string suffix = "")
    {
        if (callback == null)
        {
            BridgeConnector.Socket.On(eventName + id, () => { callback(); });
            BridgeConnector.Socket.Emit($"register-{eventName}{suffix}", id);
        }
        callback += value;
    }
    
    internal static void AddEventWithSuffix(string eventName, object id, Action callback, Action value)
    {
        AddEvent(eventName, id, callback, value, "-event");
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
    
    internal static void AddEventWithSuffix<T>(string eventName, object id, Action<T> callback, Action<T> value, Func<object, T> converter)
    {
        AddEvent(eventName, id, callback, value, converter, "-event");
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
    
    internal static void RemoveEvent(string eventName, object id, Action callback, Action value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Socket.Off(eventName + id);
    }
    
    internal static void RemoveEvent<T>(string eventName, object id, Action<T> callback, Action<T> value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Socket.Off(eventName + id);
    }

    internal static void RemoveTrayEvent(string eventName, object id, Action<TrayClickEventArgs, Rectangle> callback, Action<TrayClickEventArgs, Rectangle> value)
    {
        callback -= value;
        if (callback == null) BridgeConnector.Socket.Off(eventName + id);
    }
}