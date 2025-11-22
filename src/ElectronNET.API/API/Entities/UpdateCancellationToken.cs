namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with electron-updater 6.7.2</remarks>
    public class UpdateCancellationToken
    {
        /// <summary>
        /// Gets or sets a value indicating whether cancellation has been requested.
        /// </summary>
        public bool Cancelled { get; set; }

        /// <summary>
        /// Requests cancellation of the update process.
        /// </summary>
        public void Cancel()
        {
        }

        /// <summary>
        /// Disposes the underlying cancellation token (if applicable).
        /// </summary>
        public void Dispose()
        {
        }
    }
}