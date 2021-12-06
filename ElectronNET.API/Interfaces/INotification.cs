using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Create OS desktop notifications
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// Create OS desktop notifications
        /// </summary>
        /// <param name="notificationOptions"></param>
        void Show(NotificationOptions notificationOptions);

        /// <summary>
        /// Whether or not desktop notifications are supported on the current system.
        /// </summary>
        /// <returns></returns>
        Task<bool> IsSupportedAsync();
    }
}