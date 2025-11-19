namespace ElectronNET.API
{
    using ElectronNET.Runtime;

    /// <summary>
    ///
    /// </summary>
    public static class HybridSupport
    {
        /// <summary>
        /// Gets a value indicating whether this instance is electron active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is electron active; otherwise, <c>false</c>.
        /// </value>
        public static bool IsElectronActive
        {
            get
            {
                return ElectronHostEnvironment.Current.RuntimeController != null;
            }
        }
    }
}
