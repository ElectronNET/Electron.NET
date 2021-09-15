using System;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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
                    BridgeConnector.On<CookieRemovedResponse>("webContents-session-cookies-changed" + Id, (args) =>
                    {
                        _changed(args.cookie, args.cause, args.removed);
                    });

                    BridgeConnector.Emit("register-webContents-session-cookies-changed", Id);
                }
                _changed += value;
            }
            remove
            {
                _changed -= value;

                if (_changed == null)
                    BridgeConnector.Off("webContents-session-cookies-changed" + Id);
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
            var taskCompletionSource = new TaskCompletionSource<Cookie[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.On<Cookie[]>("webContents-session-cookies-get-completed" + guid, (cookies) =>
            {
                BridgeConnector.Off("webContents-session-cookies-get-completed" + guid);
                taskCompletionSource.SetResult(cookies);
            });

            BridgeConnector.Emit("webContents-session-cookies-get", Id, JObject.FromObject(filter, _jsonSerializer), guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public Task SetAsync(CookieDetails details)
        {
            var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.On("webContents-session-cookies-set-completed" + guid, () =>
            {
                BridgeConnector.Off("webContents-session-cookies-set-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Emit("webContents-session-cookies-set", Id, JObject.FromObject(details, _jsonSerializer), guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Removes the cookies matching url and name
        /// </summary>
        /// <param name="url">The URL associated with the cookie.</param>
        /// <param name="name">The name of cookie to remove.</param>
        /// <returns>A task which resolves when the cookie has been removed</returns>
        public Task RemoveAsync(string url, string name)
        {
            var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.On("webContents-session-cookies-remove-completed" + guid, () =>
            {
                BridgeConnector.Off("webContents-session-cookies-remove-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Emit("webContents-session-cookies-remove", Id, url, name, guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Writes any unwritten cookies data to disk.
        /// </summary>
        /// <returns>A task which resolves when the cookie store has been flushed</returns>
        public Task FlushStoreAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.On("webContents-session-cookies-flushStore-completed" + guid, () =>
            {
                BridgeConnector.Off("webContents-session-cookies-flushStore-completed" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Emit("webContents-session-cookies-flushStore", Id, guid);

            return taskCompletionSource.Task;
        }

        private static readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

    }
}