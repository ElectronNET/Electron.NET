using System;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-webContents-session-cookies-changed", Id);
                }
                _changed += value;
            }
            remove
            {
                _changed -= value;
            }
        }

        public void TriggerOnChanged(JArray jarray)
        {
            Cookie cookie = ((JArray)jarray)[0].ToObject<Cookie>();
            CookieChangedCause cause = ((JArray)jarray)[1].ToObject<CookieChangedCause>();
            bool removed = ((JArray)jarray)[2].ToObject<bool>();
            _changed(cookie, cause, removed);
        }

        private event Action<Cookie, CookieChangedCause, bool> _changed;

        /// <summary>
        /// Sends a request to get all cookies matching filter, and resolves a callack with the response.
        /// </summary>
        /// <param name="filter">
        /// </param>
        /// <returns>A task which resolves an array of cookie objects.</returns>
        public async Task<Cookie[]> GetAsync(CookieFilter filter)
        {
            var resultSignalr = await SignalrSerializeHelper.GetSignalrResultJArray("webContents-session-cookies-get", Id, JObject.FromObject(filter, _jsonSerializer));
            Cookie[] result = ((JArray)resultSignalr).ToObject<Cookie[]>();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public async Task SetAsync(CookieDetails details)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-cookies-set", Id, JObject.FromObject(details, _jsonSerializer));
        }

        /// <summary>
        /// Removes the cookies matching url and name
        /// </summary>
        /// <param name="url">The URL associated with the cookie.</param>
        /// <param name="name">The name of cookie to remove.</param>
        /// <returns>A task which resolves when the cookie has been removed</returns>
        public async Task RemoveAsync(string url, string name)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-cookies-remove", Id, url, name);
        }

        /// <summary>
        /// Writes any unwritten cookies data to disk.
        /// </summary>
        /// <returns>A task which resolves when the cookie store has been flushed</returns>
        public async Task FlushStoreAsync()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("webContents-session-cookies-flushStore", Id);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}