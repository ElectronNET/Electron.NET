using System.Threading.Tasks;

namespace ElectronNET.AspNet.Runtime
{
    internal interface IAppReadyCallbackResolver
    {
        public bool HasCallback { get; }

        public Task Invoke();
    }
}
