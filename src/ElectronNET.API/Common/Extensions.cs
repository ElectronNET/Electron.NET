namespace ElectronNET.Common
{
    using System;
    using ElectronNET.Runtime.Data;
    using ElectronNET.Runtime.Services;

    internal static class Extensions
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

        public static string LowerFirst(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToLower();
            }

            return char.ToLower(str[0]) + str.Substring(1);
        }

        public static string StripAsync(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            var pos = str.LastIndexOf("Async", StringComparison.Ordinal);

            if (pos > 0)
            {
                return str.Substring(0, pos);
            }

            return str;
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
