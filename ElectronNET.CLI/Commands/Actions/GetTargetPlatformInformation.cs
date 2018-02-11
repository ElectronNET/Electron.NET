using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
                    electronPackerPlatform = "win32";
                    break;
                case "osx":
                    netCorePublishRid = "osx-x64";
                    electronPackerPlatform = "darwin";
                    break;
                case "linux":
                    netCorePublishRid = "linux-x64";
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
                        netCorePublishRid = "win-x64";
                        electronPackerPlatform = "win32";
                    }
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        netCorePublishRid = "osx-x64";
                        electronPackerPlatform = "darwin";
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
