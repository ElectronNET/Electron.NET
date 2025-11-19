namespace ElectronNET.Runtime
{
    using System;

    /// <summary>
    /// Provides access to the singleton <see cref="IElectronHost"/> instance that is
    /// used across the application. Consumers can resolve the same instance from
    /// dependency injection via <c>services.AddSingleton(ElectronHostEnvironment.Current)</c>.
    /// </summary>
    public static class ElectronHostEnvironment
    {
        private static readonly Lazy<ElectronHost> LazyHost = new(() => new ElectronHost());

        public static IElectronHost Current => LazyHost.Value;

        internal static ElectronHost InternalHost => LazyHost.Value;
    }
}
