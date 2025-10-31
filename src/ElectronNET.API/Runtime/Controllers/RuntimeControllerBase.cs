namespace ElectronNET.Runtime.Controllers
{
    using System.Threading.Tasks;
    using ElectronNET.API;
    using ElectronNET.Runtime.Services;
    using ElectronNET.Runtime.Services.ElectronProcess;
    using ElectronNET.Runtime.Services.SocketBridge;

    internal abstract class RuntimeControllerBase : LifetimeServiceBase, IElectronNetRuntimeController
    {
        protected RuntimeControllerBase()
        {
        }

        internal abstract SocketIoFacade Socket { get; }

        internal abstract ElectronProcessBase ElectronProcess { get; }

        internal abstract SocketBridgeService SocketBridge { get; }

        protected override Task StartCore()
        {

            return Task.CompletedTask;
        }

        protected override Task StopCore()
        {
            return Task.CompletedTask;
        }

    }
}
