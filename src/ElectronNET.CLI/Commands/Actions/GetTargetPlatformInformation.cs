using System;
using System.Runtime.InteropServices;

namespace ElectronNET.CLI.Commands.Actions
{
    public static class GetTargetPlatformInformation
    {
        public struct GetTargetPlatformInformationResult
        {
            public string NetCorePublishRid { get; set; }
            public string ElectronPackerPlatform { get; set; }

        }

        public static GetTargetPlatformInformationResult Do(string desiredPlatform, string specifiedPlatfromFromCustom)
        {
            string netCorePublishRid = string.Empty;
            string electronPackerPlatform = string.Empty;

            switch (desiredPlatform)
            {
                case "win":
                    netCorePublishRid = "win-x64";
                    electronPackerPlatform = "win";
                    break;
                case "osx":
                    netCorePublishRid = "osx-x64";
                    electronPackerPlatform = "mac";
                    break;
                case "linux":
                    netCorePublishRid = "linux-x64";
                    electronPackerPlatform = "linux";
                    break;
                case "linux-arm":
                    netCorePublishRid = "linux-arm";
                    electronPackerPlatform = "linux";
                    break;
                case "custom":
                    var splittedSpecified = specifiedPlatfromFromCustom.Split(';');
                    netCorePublishRid = splittedSpecified[0];
                    electronPackerPlatform = splittedSpecified[1];
                    break;
                default:
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        netCorePublishRid = $"win-x{(Environment.Is64BitOperatingSystem ? "64" : "86")}";
                        electronPackerPlatform = "win";
                    }
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        netCorePublishRid = "osx-x64";
                        electronPackerPlatform = "mac";
                    }
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        netCorePublishRid = "linux-x64";
                        electronPackerPlatform = "linux";
                    }

                    break;
            }

            return new GetTargetPlatformInformationResult()
            {
                ElectronPackerPlatform = electronPackerPlatform,
                NetCorePublishRid = netCorePublishRid
            };
        }
    }
}
