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
                    BridgeConnector.Socket.On("webContents-session-cookies-changed" + Id, (args) =>
                    {
                        Cookie cookie = ((JArray)args)[0].ToObject<Cookie>();
                        CookieChangedCause cause = ((JArray)args)[1].ToObject<CookieChangedCause>();
                        bool removed = ((JArray)args)[2].ToObject<bool>();
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
                    BridgeConnector.Socket.Off("webContents-session-cookies-changed" + Id);
            }
        }

        private event Action<Cookie, CookieChangedCause, bool> _changed;

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}