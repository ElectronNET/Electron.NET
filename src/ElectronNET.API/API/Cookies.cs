using ElectronNET.API.Entities;
using ElectronNET.API.Serialization;
using System;
using System.Text.Json;

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
                    BridgeConnector.Socket.Off("webContents-session-cookies-changed" + Id);
            }
        }

        private event Action<Cookie, CookieChangedCause, bool> _changed;


    }
}
