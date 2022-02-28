using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Perform copy and paste operations on the system clipboard.
    /// </summary>
    public sealed class Clipboard
    {
        private static Clipboard _clipboard;
        private static object _syncRoot = new object();

        internal Clipboard() { }

        internal static Clipboard Instance
        {
            get
            {
                if (_clipboard == null)
                {
                    lock (_syncRoot)
                    {
                        if (_clipboard == null)
                        {
                            _clipboard = new Clipboard();
                        }
                    }
                }

                return _clipboard;
            }
        }

        /// <summary>
        /// Read the content in the clipboard as plain text.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The content in the clipboard as plain text.</returns>
        public async Task<string> ReadTextAsync(string type = "")
        {
            return (await SignalrSerializeHelper.GetSignalrResultStringParameter("clipboard-readText", type));
        }

        /// <summary>
        /// Writes the text into the clipboard as plain text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public async void WriteText(string text, string type = "")
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("clipboard-writeText", text, type);
        }

        /// <summary>
        /// The content in the clipboard as markup.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<string> ReadHTMLAsync(string type = "")
        {
            return (await SignalrSerializeHelper.GetSignalrResultStringParameter("clipboard-readHTML", type));
        }

        /// <summary>
        /// Writes markup to the clipboard.
        /// </summary>
        /// <param name="markup"></param>
        /// <param name="type"></param>
        public async void WriteHTML(string markup, string type = "")
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("clipboard-writeHTML", markup, type);
        }

        /// <summary>
        /// The content in the clipboard as RTF.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<string> ReadRTFAsync(string type = "")
        {
            return (await SignalrSerializeHelper.GetSignalrResultStringParameter("clipboard-readRTF", type));
        }

        /// <summary>
        /// Writes the text into the clipboard in RTF.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public async void WriteRTF(string text, string type = "")
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("clipboard-writeHTML", text, type);
        }

        /// <summary>
        /// Returns an Object containing title and url keys representing 
        /// the bookmark in the clipboard. The title and url values will 
        /// be empty strings when the bookmark is unavailable.
        /// </summary>
        /// <returns></returns>
        public async Task<ReadBookmark> ReadBookmarkAsync()
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("clipboard-readBookmark");
            return ((JObject)signalrResult).ToObject<ReadBookmark>();
        }

        /// <summary>
        /// Writes the title and url into the clipboard as a bookmark.
        /// 
        /// Note: Most apps on Windows don’t support pasting bookmarks
        /// into them so you can use clipboard.write to write both a 
        /// bookmark and fallback text to the clipboard.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="type"></param>
        public async void WriteBookmark(string title, string url, string type = "")
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("clipboard-writeBookmark", title, url, type);
        }

        /// <summary>
        /// macOS: The text on the find pasteboard. This method uses synchronous IPC
        /// when called from the renderer process. The cached value is reread from the
        /// find pasteboard whenever the application is activated.
        /// </summary>
        /// <returns></returns>
        public async Task<string> ReadFindTextAsync()
        {
            return (await SignalrSerializeHelper.GetSignalrResultString("clipboard-readFindText"));
        }

        /// <summary>
        /// macOS: Writes the text into the find pasteboard as plain text. This method uses 
        /// synchronous IPC when called from the renderer process.
        /// </summary>
        /// <param name="text"></param>
        public async void WriteFindText(string text)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("clipboard-writeFindText", text);
        }

        /// <summary>
        /// Clears the clipboard content.
        /// </summary>
        /// <param name="type"></param>
        public async void Clear(string type = "")
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("clipboard-clear", type);
        }

        /// <summary>
        /// An array of supported formats for the clipboard type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<string[]> AvailableFormatsAsync(string type = "")
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJArray("clipboard-availableFormats");
            return ((JArray)signalrResult).ToObject<string[]>();
        }

        /// <summary>
        /// Writes data to the clipboard.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        public async void Write(Data data, string type = "")
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("clipboard-write", JObject.FromObject(data, _jsonSerializer), type);
        }

        /// <summary>
        /// Reads an image from the clipboard.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<NativeImage> ReadImageAsync(string type = "")
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("clipboard-readImage", type);
            return ((JObject)signalrResult).ToObject<NativeImage>();
        }
        
        /// <summary>
        /// Writes an image to the clipboard.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        public async Task WriteImage(NativeImage image, string type = "")
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("clipboard-writeImage", JsonConvert.SerializeObject(image), type);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}