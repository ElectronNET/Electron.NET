using System;
using System.Threading.Tasks;

namespace ElectronNET.AspNet.Runtime
{
    internal class AppReadyCallbackResolver : IAppReadyCallbackResolver
    {
        private readonly Func<Task> _callback;

        public AppReadyCallbackResolver()
        { }

        public AppReadyCallbackResolver(Func<Task> callback)
        {
            _callback = callback;
        }

        public AppReadyCallbackResolver(string[] args, Func<string[], Task> callback)
        {
            _callback = () => callback.Invoke(args);
        }

        public AppReadyCallbackResolver(IServiceProvider serviceProvider, Func<IServiceProvider, Task> callback)
        {
            _callback = () => callback.Invoke(serviceProvider);
        }

        public AppReadyCallbackResolver(IServiceProvider serviceProvider, string[] args, Func<IServiceProvider, string[], Task> callback)
        {
            _callback = () => callback.Invoke(serviceProvider, args);
        }

        public bool HasCallback => _callback != null;

        public Task Invoke() => _callback?.Invoke() ?? Task.CompletedTask;
    }
}
