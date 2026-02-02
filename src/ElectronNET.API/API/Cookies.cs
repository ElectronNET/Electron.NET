using ElectronNET.API.Entities;
using ElectronNET.API.Serialization;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Query and modify a session's cookies.
    /// </summary>
    public class Cookies
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; private set; }

        internal Cookies(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Emitted when a cookie is changed because it was added, edited, removed, or expired.
        /// </summary>
        public event Action<Cookie, CookieChangedCause, bool> OnChanged
        {
            add
            {
                if (_changed == null)
                {
                    BridgeConnector.Socket.On<JsonElement>("webContents-session-cookies-changed" + Id, (args) =>
                    {
                        var e = args.EnumerateArray().GetEnumerator();
                        e.MoveNext();
                        var cookie = e.Current.Deserialize<Cookie>(ElectronJson.Options);
                        e.MoveNext();
                        var cause = e.Current.Deserialize<CookieChangedCause>(ElectronJson.Options);
                        e.MoveNext();
                        var removed = e.Current.GetBoolean();
                        _changed(cookie, cause, removed);
                    });

                    BridgeConnector.Socket.Emit("register-webContents-session-cookies-changed", Id);
                }

                _changed += value;
            }
            remove
            {
                _changed -= value;

                if (_changed == null)
                {
                    BridgeConnector.Socket.Off("webContents-session-cookies-changed" + Id);
                }
            }
        }

        private event Action<Cookie, CookieChangedCause, bool> _changed;

        

        /// <summary>
        /// Sends a request to get all cookies matching filter, and resolves a callack with the response.
        /// </summary>
        /// <param name="filter">
        /// </param>
        /// <returns>A task which resolves an array of cookie objects.</returns>
        public Task<Cookie[]> GetAsync(CookieFilter filter)
        {
            var tcs = new TaskCompletionSource<Cookie[]>();
            var guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<Cookie[]>("webContents-session-cookies-get-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-cookies-get", Id, filter, guid);

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public Task SetAsync(CookieDetails details)
        {
            var tcs = new TaskCompletionSource<object>();
            var guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<object>("webContents-session-cookies-set-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-cookies-set", Id, details, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Removes the cookies matching url and name
        /// </summary>
        /// <param name="url">The URL associated with the cookie.</param>
        /// <param name="name">The name of cookie to remove.</param>
        /// <returns>A task which resolves when the cookie has been removed</returns>
        public Task RemoveAsync(string url, string name)
        {
            var tcs = new TaskCompletionSource<object>();
            var guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<object>("webContents-session-cookies-remove-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-cookies-remove", Id, url, name, guid);

            return tcs.Task;
        }

        /// <summary>
        /// Writes any unwritten cookies data to disk.
        /// </summary>
        /// <returns>A task which resolves when the cookie store has been flushed</returns>
        public Task FlushStoreAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            var guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.Once<object>("webContents-session-cookies-flushStore-completed" + guid, tcs.SetResult);
            BridgeConnector.Socket.Emit("webContents-session-cookies-flushStore", Id, guid);

            return tcs.Task;
        }
    }
}