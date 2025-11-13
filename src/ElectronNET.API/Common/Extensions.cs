using System.Globalization;
using System.Text.RegularExpressions;
using System;
using ElectronNET.Runtime.Data;
using ElectronNET.Runtime.Services;

namespace ElectronNET.Common
{
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
        
        public static string StripOn(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) || !str.StartsWith("On", StringComparison.Ordinal))
            {
                return str;
            }
            
            return str.Substring(2);
        }

        public static string ToDashedEventName(this string str)
        {
            return string.Join("-", Regex.Split(str.StripOn(), "(?<!^)(?=[A-Z])")).ToLower(CultureInfo.InvariantCulture);
        }
        
        public static string ToCamelCaseEventName(this string str)
        {
            return str.StripOn().LowerFirst();
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
