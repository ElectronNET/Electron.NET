using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Perform copy and paste operations on the system clipboard.
    /// </summary>
    public sealed class Clipboard
    {
        private static Clipboard _clipboard;
        private static object _syncRoot = new Object();

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
        public Task<string> ReadTextAsync(string type = "")
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("clipboard-readText-Completed", (text) =>
            {
                BridgeConnector.Socket.Off("clipboard-readText-Completed");

                taskCompletionSource.SetResult(text.ToString());
            });

            BridgeConnector.Socket.Emit("clipboard-readText", type);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Writes the text into the clipboard as plain text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public void WriteText(string text, string type = "")
        {
            BridgeConnector.Socket.Emit("clipboard-writeText", text, type);
        }

        /// <summary>
        /// The content in the clipboard as markup.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<string> ReadHTMLAsync(string type = "")
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("clipboard-readHTML-Completed", (text) =>
            {
                BridgeConnector.Socket.Off("clipboard-readHTML-Completed");

                taskCompletionSource.SetResult(text.ToString());
            });

            BridgeConnector.Socket.Emit("clipboard-readHTML", type);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Writes markup to the clipboard.
        /// </summary>
        /// <param name="markup"></param>
        /// <param name="type"></param>
        public void WriteHTML(string markup, string type = "")
        {
            BridgeConnector.Socket.Emit("clipboard-writeHTML", markup, type);
        }

        /// <summary>
        /// The content in the clipboard as RTF.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<string> ReadRTFAsync(string type = "")
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("clipboard-readRTF-Completed", (text) =>
            {
                BridgeConnector.Socket.Off("clipboard-readRTF-Completed");

                taskCompletionSource.SetResult(text.ToString());
            });

            BridgeConnector.Socket.Emit("clipboard-readRTF", type);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Writes the text into the clipboard in RTF.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public void WriteRTF(string text, string type = "")
        {
            BridgeConnector.Socket.Emit("clipboard-writeHTML", text, type);
        }

        /// <summary>
        /// Returns an Object containing title and url keys representing 
        /// the bookmark in the clipboard. The title and url values will 
        /// be empty strings when the bookmark is unavailable.
        /// </summary>
        /// <returns></returns>
        public Task<ReadBookmark> ReadBookmarkAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<ReadBookmark>();

            BridgeConnector.Socket.On("clipboard-readBookmark-Completed", (bookmark) =>
            {
                BridgeConnector.Socket.Off("clipboard-readBookmark-Completed");

                taskCompletionSource.SetResult(((JObject)bookmark).ToObject<ReadBookmark>());
            });

            BridgeConnector.Socket.Emit("clipboard-readBookmark");

            return taskCompletionSource.Task;
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
        public void WriteBookmark(string title, string url, string type = "")
        {
            BridgeConnector.Socket.Emit("clipboard-writeBookmark", title, url, type);
        }

        /// <summary>
        /// macOS: The text on the find pasteboard. This method uses synchronous IPC
        /// when called from the renderer process. The cached value is reread from the
        /// find pasteboard whenever the application is activated.
        /// </summary>
        /// <returns></returns>
        public Task<string> ReadFindTextAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("clipboard-readFindText-Completed", (text) =>
            {
                BridgeConnector.Socket.Off("clipboard-readFindText-Completed");

                taskCompletionSource.SetResult(text.ToString());
            });

            BridgeConnector.Socket.Emit("clipboard-readFindText");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// macOS: Writes the text into the find pasteboard as plain text. This method uses 
        /// synchronous IPC when called from the renderer process.
        /// </summary>
        /// <param name="text"></param>
        public void WriteFindText(string text)
        {
            BridgeConnector.Socket.Emit("clipboard-writeFindText", text);
        }

        /// <summary>
        /// Clears the clipboard content.
        /// </summary>
        /// <param name="type"></param>
        public void Clear(string type = "")
        {
            BridgeConnector.Socket.Emit("clipboard-clear", type);
        }

        /// <summary>
        /// An array of supported formats for the clipboard type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<string[]> AvailableFormatsAsync(string type = "")
        {
            var taskCompletionSource = new TaskCompletionSource<string[]>();

            BridgeConnector.Socket.On("clipboard-availableFormats-Completed", (formats) =>
            {
                BridgeConnector.Socket.Off("clipboard-availableFormats-Completed");

                taskCompletionSource.SetResult(((JArray)formats).ToObject<string[]>());
            });

            BridgeConnector.Socket.Emit("clipboard-availableFormats", type);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Writes data to the clipboard.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        public void Write(Data data, string type = "")
        {
            BridgeConnector.Socket.Emit("clipboard-write", JObject.FromObject(data, _jsonSerializer), type);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}