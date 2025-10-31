namespace ElectronNET.Runtime.Services.ElectronProcess
{
    using System.ComponentModel;

    /// <summary>
    /// Manages the Electron app process.
    /// </summary>
    [Localizable(false)]
    internal abstract class ElectronProcessBase : LifetimeServiceBase
    {
        protected ElectronProcessBase()
        {
        }
    }
}