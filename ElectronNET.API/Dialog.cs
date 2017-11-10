using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Display native system dialogs for opening and saving files, alerting, etc.
    /// </summary>
    public sealed class Dialog
    {
        private static Dialog _dialog;
        private static object _syncRoot = new Object();

        internal Dialog() { }

        internal static Dialog Instance
        {
            get
            {
                if (_dialog == null)
                {
                    lock (_syncRoot)
                    {
                        if(_dialog == null)
                        {
                            _dialog = new Dialog();
                        }
                    }
                }

                return _dialog;
            }
        }

        /// <summary>
        /// Note: On Windows and Linux an open dialog can not be both a file selector 
        /// and a directory selector, so if you set properties to ['openFile', 'openDirectory'] 
        /// on these platforms, a directory selector will be shown.
        /// </summary>
        /// <param name="browserWindow">The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.</param>
        /// <param name="options"></param>
        /// <returns>An array of file paths chosen by the user</returns>
        public Task<string[]> ShowOpenDialogAsync(BrowserWindow browserWindow, OpenDialogOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<string[]>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("showOpenDialogComplete" + guid, (filePaths) =>
            {
                BridgeConnector.Socket.Off("showOpenDialogComplete" + guid);

                var result = ((JArray)filePaths).ToObject<string[]>();
                taskCompletionSource.SetResult(result);
            });


            BridgeConnector.Socket.Emit("showOpenDialog",
            JObject.FromObject(browserWindow, _jsonSerializer),
            JObject.FromObject(options, _jsonSerializer),
            guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Dialog for save files.
        /// </summary>
        /// <param name="browserWindow">The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.</param>
        /// <param name="options"></param>
        /// <returns>Returns String, the path of the file chosen by the user, if a callback is provided it returns an empty string.</returns>
        public Task<string> ShowSaveDialogAsync(BrowserWindow browserWindow, SaveDialogOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("showSaveDialogComplete" + guid, (filename) =>
            {
                BridgeConnector.Socket.Off("showSaveDialogComplete" + guid);

                taskCompletionSource.SetResult(filename.ToString());
            });

            BridgeConnector.Socket.Emit("showSaveDialog",
            JObject.FromObject(browserWindow, _jsonSerializer),
            JObject.FromObject(options, _jsonSerializer),
            guid);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Shows a message box, it will block the process until the message box is closed.
        /// It returns the index of the clicked button. The browserWindow argument allows
        /// the dialog to attach itself to a parent window, making it modal. If a callback
        /// is passed, the dialog will not block the process.The API call will be
        /// asynchronous and the result will be passed via callback(response).
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The API call will be asynchronous and the result will be passed via MessageBoxResult.</returns>
        public async Task<MessageBoxResult> ShowMessageBoxAsync(string message)
        {
            return await ShowMessageBoxAsync(null, new MessageBoxOptions(message));
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
        /// <param name="message"></param>
        /// <returns>The API call will be asynchronous and the result will be passed via MessageBoxResult.</returns>
        public async Task<MessageBoxResult> ShowMessageBoxAsync(BrowserWindow browserWindow, string message)
        {
            return await ShowMessageBoxAsync(browserWindow, new MessageBoxOptions(message));
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
            var guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("showMessageBoxComplete" + guid, (args) =>
            {
                BridgeConnector.Socket.Off("showMessageBoxComplete" + guid);

                var result = ((JArray)args);

                taskCompletionSource.SetResult(new MessageBoxResult
                {
                    Response = (int)result.First,
                    CheckboxChecked = (bool)result.Last
                });

            });

            if (browserWindow == null)
            {
                BridgeConnector.Socket.Emit("showMessageBox", JObject.FromObject(messageBoxOptions, _jsonSerializer), guid);
            } else
            {
                BridgeConnector.Socket.Emit("showMessageBox", 
                    JObject.FromObject(browserWindow, _jsonSerializer),
                    JObject.FromObject(messageBoxOptions, _jsonSerializer),
                    guid);
            }

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Displays a modal dialog that shows an error message.
        /// 
        /// This API can be called safely before the ready event the app module emits, 
        /// it is usually used to report errors in early stage of startup.If called 
        /// before the app readyevent on Linux, the message will be emitted to stderr, 
        /// and no GUI dialog will appear.
        /// </summary>
        /// <param name="title">The title to display in the error box.</param>
        /// <param name="content">The text content to display in the error box.</param>
        public void ShowErrorBox(string title, string content)
        {
            BridgeConnector.Socket.Emit("showErrorBox", title, content);
        }

        /// <summary>
        /// On macOS, this displays a modal dialog that shows a message and certificate information,
        /// and gives the user the option of trusting/importing the certificate. If you provide a 
        /// browserWindow argument the dialog will be attached to the parent window, making it modal.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task ShowCertificateTrustDialogAsync(CertificateTrustDialogOptions options)
        {
            return ShowCertificateTrustDialogAsync(null, options);
        }

        /// <summary>
        /// On macOS, this displays a modal dialog that shows a message and certificate information,
        /// and gives the user the option of trusting/importing the certificate. If you provide a 
        /// browserWindow argument the dialog will be attached to the parent window, making it modal.
        /// </summary>
        /// <param name="browserWindow"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task ShowCertificateTrustDialogAsync(BrowserWindow browserWindow, CertificateTrustDialogOptions options)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            string guid = Guid.NewGuid().ToString();

            BridgeConnector.Socket.On("showCertificateTrustDialogComplete" + guid, () =>
            {
                BridgeConnector.Socket.Off("showCertificateTrustDialogComplete" + guid);
                taskCompletionSource.SetResult(null);
            });

            BridgeConnector.Socket.Emit("showCertificateTrustDialog",
                JObject.FromObject(browserWindow, _jsonSerializer),
                JObject.FromObject(options, _jsonSerializer),
                guid);

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
