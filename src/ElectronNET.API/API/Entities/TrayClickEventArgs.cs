namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public class TrayClickEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether [alt key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [alt key]; otherwise, <c>false</c>.
        /// </value>
        public bool AltKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [shift key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [shift key]; otherwise, <c>false</c>.
        /// </value>
        public bool ShiftKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [control key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [control key]; otherwise, <c>false</c>.
        /// </value>
        public bool CtrlKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [meta key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [meta key]; otherwise, <c>false</c>.
        /// </value>
        public bool MetaKey { get; set; }
    }
}