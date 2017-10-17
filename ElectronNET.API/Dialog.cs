using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    public sealed class Dialog
    {
        private static Dialog _dialog;

        internal Dialog() { }

        internal static Dialog Instance
        {
            get
            {
                if (_dialog == null)
                {
                    _dialog = new Dialog();
                }

                return _dialog;
            }
        }

        /// <summary>
        /// Note: On Windows and Linux an open dialog can not be both a file selector 
        /// and a directory selector, so if you set properties to ['openFile', 'openDirectory'] 
        /// on these platforms, a directory selector will be shown.
        /// </summary>
        /// <param name="browserWindow"></param>
        /// <param name="options"></param>
        /// <returns>An array of file paths chosen by the user</returns>
        public Task<string[]> ShowOpenDialogAsync(BrowserWindow browserWindow, OpenDialogOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<string[]>();

            BridgeConnector.Socket.On("showOpenDialogComplete", (filePaths) =>
            {
                BridgeConnector.Socket.Off("showOpenDialogComplete");

                var result = ((JArray)filePaths).ToObject<string[]>();
                taskCompletionSource.SetResult(result);
            });

                BridgeConnector.Socket.Emit("showOpenDialog", 
                JObject.FromObject(browserWindow, _jsonSerializer), 
                JObject.FromObject(options, _jsonSerializer));

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Shows a message box, it will block the process until the message box is closed.
        /// It returns the index of the clicked button. The browserWindow argument allows
        /// the dialog to attach itself to a parent window, making it modal. If a callback
        /// is passed, the dialog will not block the process.The API call will be
        /// asynchronous and the result will be passed via callback(response).
        /// </summary>
        /// <param name="messageBoxOptions"></param>
        /// <returns>The API call will be asynchronous and the result will be passed via MessageBoxResult.</returns>
        public async Task<MessageBoxResult> ShowMessageBoxAsync(MessageBoxOptions messageBoxOptions)
        {
            return await ShowMessageBoxAsync(null, messageBoxOptions);
        }

        /// <summary>
        /// Shows a message box, it will block the process until the message box is closed.
        /// It returns the index of the clicked button. If a callback
        /// is passed, the dialog will not block the process.
        /// </summary>
        /// <param name="browserWindow">The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.</param>
        /// <param name="messageBoxOptions"></param>
        /// <returns>The API call will be asynchronous and the result will be passed via MessageBoxResult.</returns>
        public Task<MessageBoxResult> ShowMessageBoxAsync(BrowserWindow browserWindow, MessageBoxOptions messageBoxOptions)
        {
            var taskCompletionSource = new TaskCompletionSource<MessageBoxResult>();

            BridgeConnector.Socket.On("showMessageBoxComplete", (args) =>
            {
                BridgeConnector.Socket.Off("showMessageBoxComplete");

                var result = ((JArray)args);

                taskCompletionSource.SetResult(new MessageBoxResult
                {
                    Response = (int)result.First,
                    CheckboxChecked = (bool)result.Last
                });

            });

            if (browserWindow == null)
            {
                BridgeConnector.Socket.Emit("showMessageBox", JObject.FromObject(messageBoxOptions, _jsonSerializer));
            } else
            {
                BridgeConnector.Socket.Emit("showMessageBox", 
                    JObject.FromObject(browserWindow, _jsonSerializer),
                    JObject.FromObject(messageBoxOptions, _jsonSerializer));
            }

            return taskCompletionSource.Task;
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
