using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Perform copy and paste operations on the system clipboard.
    /// </summary>
    public interface IClipboard
    {
        /// <summary>
        /// Read the content in the clipboard as plain text.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The content in the clipboard as plain text.</returns>
        Task<string> ReadTextAsync(string type = "");

        /// <summary>
        /// Writes the text into the clipboard as plain text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        void WriteText(string text, string type = "");

        /// <summary>
        /// The content in the clipboard as markup.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<string> ReadHTMLAsync(string type = "");

        /// <summary>
        /// Writes markup to the clipboard.
        /// </summary>
        /// <param name="markup"></param>
        /// <param name="type"></param>
        void WriteHTML(string markup, string type = "");

        /// <summary>
        /// The content in the clipboard as RTF.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<string> ReadRTFAsync(string type = "");

        /// <summary>
        /// Writes the text into the clipboard in RTF.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        void WriteRTF(string text, string type = "");

        /// <summary>
        /// Returns an Object containing title and url keys representing 
        /// the bookmark in the clipboard. The title and url values will 
        /// be empty strings when the bookmark is unavailable.
        /// </summary>
        /// <returns></returns>
        Task<ReadBookmark> ReadBookmarkAsync();

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
        void WriteBookmark(string title, string url, string type = "");

        /// <summary>
        /// macOS: The text on the find pasteboard. This method uses synchronous IPC
        /// when called from the renderer process. The cached value is reread from the
        /// find pasteboard whenever the application is activated.
        /// </summary>
        /// <returns></returns>
        Task<string> ReadFindTextAsync();

        /// <summary>
        /// macOS: Writes the text into the find pasteboard as plain text. This method uses 
        /// synchronous IPC when called from the renderer process.
        /// </summary>
        /// <param name="text"></param>
        void WriteFindText(string text);

        /// <summary>
        /// Clears the clipboard content.
        /// </summary>
        /// <param name="type"></param>
        void Clear(string type = "");

        /// <summary>
        /// An array of supported formats for the clipboard type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<string[]> AvailableFormatsAsync(string type = "");

        /// <summary>
        /// Writes data to the clipboard.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        void Write(Data data, string type = "");

        /// <summary>
        /// Reads an image from the clipboard.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<NativeImage> ReadImageAsync(string type = "");

        /// <summary>
        /// Writes an image to the clipboard.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="type"></param>
        void WriteImage(NativeImage image, string type = "");
    }
}