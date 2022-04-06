using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Display native system dialogs for opening and saving files, alerting, etc.
    /// </summary>
    public interface IDialog
    {
        /// <summary>
        /// Note: On Windows and Linux an open dialog can not be both a file selector 
        /// and a directory selector, so if you set properties to ['openFile', 'openDirectory'] 
        /// on these platforms, a directory selector will be shown.
        /// </summary>
        /// <param name="browserWindow">The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.</param>
        /// <param name="options"></param>
        /// <returns>An array of file paths chosen by the user</returns>
        Task<string[]> ShowOpenDialogAsync(BrowserWindow browserWindow, OpenDialogOptions options);

        /// <summary>
        /// Dialog for save files.
        /// </summary>
        /// <param name="browserWindow">The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.</param>
        /// <param name="options"></param>
        /// <returns>Returns String, the path of the file chosen by the user, if a callback is provided it returns an empty string.</returns>
        Task<string> ShowSaveDialogAsync(BrowserWindow browserWindow, SaveDialogOptions options);

        /// <summary>
        /// Shows a message box, it will block the process until the message box is closed.
        /// It returns the index of the clicked button. The browserWindow argument allows
        /// the dialog to attach itself to a parent window, making it modal. If a callback
        /// is passed, the dialog will not block the process.The API call will be
        /// asynchronous and the result will be passed via callback(response).
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The API call will be asynchronous and the result will be passed via MessageBoxResult.</returns>
        Task<MessageBoxResult> ShowMessageBoxAsync(string message);

        /// <summary>
        /// Shows a message box, it will block the process until the message box is closed.
        /// It returns the index of the clicked button. The browserWindow argument allows
        /// the dialog to attach itself to a parent window, making it modal. If a callback
        /// is passed, the dialog will not block the process.The API call will be
        /// asynchronous and the result will be passed via callback(response).
        /// </summary>
        /// <param name="messageBoxOptions"></param>
        /// <returns>The API call will be asynchronous and the result will be passed via MessageBoxResult.</returns>
        Task<MessageBoxResult> ShowMessageBoxAsync(MessageBoxOptions messageBoxOptions);

        /// <summary>
        /// Shows a message box, it will block the process until the message box is closed.
        /// It returns the index of the clicked button. If a callback
        /// is passed, the dialog will not block the process.
        /// </summary>
        /// <param name="browserWindow">The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.</param>
        /// <param name="message"></param>
        /// <returns>The API call will be asynchronous and the result will be passed via MessageBoxResult.</returns>
        Task<MessageBoxResult> ShowMessageBoxAsync(BrowserWindow browserWindow, string message);

        /// <summary>
        /// Shows a message box, it will block the process until the message box is closed.
        /// It returns the index of the clicked button. If a callback
        /// is passed, the dialog will not block the process.
        /// </summary>
        /// <param name="browserWindow">The browserWindow argument allows the dialog to attach itself to a parent window, making it modal.</param>
        /// <param name="messageBoxOptions"></param>
        /// <returns>The API call will be asynchronous and the result will be passed via MessageBoxResult.</returns>
        Task<MessageBoxResult> ShowMessageBoxAsync(BrowserWindow browserWindow, MessageBoxOptions messageBoxOptions);

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
        void ShowErrorBox(string title, string content);

        /// <summary>
        /// On macOS, this displays a modal dialog that shows a message and certificate information,
        /// and gives the user the option of trusting/importing the certificate. If you provide a 
        /// browserWindow argument the dialog will be attached to the parent window, making it modal.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task ShowCertificateTrustDialogAsync(CertificateTrustDialogOptions options);

        /// <summary>
        /// On macOS, this displays a modal dialog that shows a message and certificate information,
        /// and gives the user the option of trusting/importing the certificate. If you provide a 
        /// browserWindow argument the dialog will be attached to the parent window, making it modal.
        /// </summary>
        /// <param name="browserWindow"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task ShowCertificateTrustDialogAsync(BrowserWindow browserWindow, CertificateTrustDialogOptions options);
    }
}