namespace ElectronNET.Common
{
    using System;
    using System.Collections.Immutable;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services;

    public static class Extensions
    {
        public static bool IsUnpackaged(this StartupMethod method)
        {
            switch (method)
            {
                case StartupMethod.UnpackedElectronFirst:
                case StartupMethod.UnpackedDotnetFirst:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsReady(this LifetimeServiceBase service)
        {
            return service != null && service.State == LifetimeState.Ready;
        }

        public static bool IsNotStopped(this LifetimeServiceBase service)
        {
            return service != null && service.State != LifetimeState.Stopped;
        }

        public static bool IsNullOrStopped(this LifetimeServiceBase service)
        {
            return service == null || service.State == LifetimeState.Stopped;
        }
    }
}
