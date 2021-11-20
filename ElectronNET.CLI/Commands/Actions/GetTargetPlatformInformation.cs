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
                case "osx-arm64":
                    netCorePublishRid = "osx-arm64";
                    electronPackerPlatform = "darwin-arm64";
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
                        if (RuntimeInformation.OSArchitecture.Equals(Architecture.Arm64))
                        {
                            //Apple Silicon Mac:
                            netCorePublishRid = "osx-arm64";
                            electronPackerPlatform = "darwin-arm64";
                        }
                        else{
                            //Intel Mac:
                            netCorePublishRid = "osx-x64";
                            electronPackerPlatform = "mac";
                        }
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
