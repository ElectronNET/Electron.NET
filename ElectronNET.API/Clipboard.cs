using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using ElectronNET.API.Interfaces;

namespace ElectronNET.API
{
    /// <summary>
    /// Perform copy and paste operations on the system clipboard.
    /// </summary>
    public sealed class Clipboard : IClipboard
    {
        private static Clipboard _clipboard;
        private static readonly object _syncRoot = new();

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
        public Task<string> ReadTextAsync(string type = "") => BridgeConnector.OnResult<string>("clipboard-readText", "clipboard-readText-Completed", type);

        /// <summary>
        /// Writes the text into the clipboard as plain text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public void WriteText(string text, string type = "")
        {
            BridgeConnector.Emit("clipboard-writeText", text, type);
        }

        /// <summary>
        /// The content in the clipboard as markup.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<string> ReadHTMLAsync(string type = "") => BridgeConnector.OnResult<string>("clipboard-readHTML", "clipboard-readHTML-Completed", type);

        /// <summary>
        /// Writes markup to the clipboard.
        /// </summary>
        /// <param name="markup"></param>
        /// <param name="type"></param>
        public void WriteHTML(string markup, string type = "")
        {
            BridgeConnector.Emit("clipboard-writeHTML", markup, type);
        }

        /// <summary>
        /// The content in the clipboard as RTF.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<string> ReadRTFAsync(string type = "") => BridgeConnector.OnResult<string>("clipboard-readRTF", "clipboard-readRTF-Completed", type);


        /// <summary>
        /// Writes the text into the clipboard in RTF.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public void WriteRTF(string text, string type = "")
        {
            BridgeConnector.Emit("clipboard-writeHTML", text, type);
        }

        /// <summary>
        /// Returns an Object containing title and url keys representing 
        /// the bookmark in the clipboard. The title and url values will 
        /// be empty strings when the bookmark is unavailable.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")] 
        public Task<ReadBookmark> ReadBookmarkAsync() => BridgeConnector.OnResult<ReadBookmark>("clipboard-readBookmark", "clipboard-readBookmark-Completed");

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
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public void WriteBookmark(string title, string url, string type = "")
        {
            BridgeConnector.Emit("clipboard-writeBookmark", title, url, type);
        }

        /// <summary>
        /// macOS: The text on the find pasteboard. This method uses synchronous IPC
        /// when called from the renderer process. The cached value is reread from the
        /// find pasteboard whenever the application is activated.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("macos")] 
        public Task<string> ReadFindTextAsync() => BridgeConnector.OnResult<string>("clipboard-readFindText", "clipboard-readFindText-Completed");

        /// <summary>
        /// macOS: Writes the text into the find pasteboard as plain text. This method uses 
        /// synchronous IPC when called from the renderer process.
        /// </summary>
        /// <param name="text"></param>
        [SupportedOSPlatform("macos")]
        public void WriteFindText(string text)
        {
            BridgeConnector.Emit("clipboard-writeFindText", text);
        }

        /// <summary>
        /// Clears the clipboard content.
        /// </summary>
        /// <param name="type"></param>
        public void Clear(string type = "")
        {
            BridgeConnector.Emit("clipboard-clear", type);
        }

        /// <summary>
        /// An array of supported formats for the clipboard type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<string[]> AvailableFormatsAsync(string type = "") => BridgeConnector.OnResult<string[]>("clipboard-availableFormats", "clipboard-availableFormats-Completed", type);

        /// <summary>
        /// Writes data to the clipboard.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        public void Write(Data data, string type = "")
        {
            BridgeConnector.Emit("clipboard-write", JObject.FromObject(data, _jsonSerializer), type);
        }

        /// <summary>
        /// Reads an image from the clipboard.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<NativeImage> ReadImageAsync(string type = "") => BridgeConnector.OnResult<NativeImage>("clipboard-readImage", "clipboard-readImage-Completed", type);
        
        /// <summary>
        /// Writes an image to the clipboard.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        public void WriteImage(NativeImage image, string type = "")
        {
            BridgeConnector.Emit("clipboard-writeImage", JsonConvert.SerializeObject(image), type);
        }

        private static readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}