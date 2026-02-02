namespace ElectronNET.Runtime.Helpers
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Helper class for debugger detection with lazy initialization.
    /// </summary>
    internal static class DebuggerHelper
    {
        /// <summary>
        /// Gets whether a debugger is attached. This value is cached for performance.
        /// </summary>
        public static bool IsAttached => _isAttached.Value;

        private static readonly Lazy<bool> _isAttached = new Lazy<bool>(() => Debugger.IsAttached);
    }
}
