using System;
using System.Diagnostics;
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
                    electronPackerPlatform = "mac";

                    //Check to see if .net 6 is installed:
                    if (!Dotnet6Installed())
                    {
                        throw new ArgumentException("You are using a dotnet version older than dotnet 6. Compiling for osx-arm64 requires that dotnet 6 or greater is installed and targeted by your project.", "osx-arm64");
                    }

                    //Warn for .net 6 targeting:
                    Console.WriteLine("Please ensure that your project targets .net 6 or greater. Otherwise you may experience an error compiling for osx-arm64.");
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
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        if (RuntimeInformation.OSArchitecture.Equals(Architecture.Arm64) && Dotnet6Installed())
                        {
                            //Warn for .net 6 targeting:
                            Console.WriteLine("Please ensure that your project targets .net 6. Otherwise you may experience an error.");

                            //Apple Silicon Mac:
                            netCorePublishRid = "osx-arm64";
                            electronPackerPlatform = "mac";
                        }
                        else
                        {
                            //Intel Mac:
                            netCorePublishRid = "osx-x64";
                            electronPackerPlatform = "mac";
                        }
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
        /// <summary>
        /// Checks to see if dotnet 6 or greater is installed.
        /// Required for MacOS arm targeting.
        /// Note that an error may still occur if the project being compiled does not target dotnet 6 or greater. 
        /// </summary>
        /// <returns>
        /// Returns true if dotnet 6 or greater is installed.
        /// </returns>
        private static bool Dotnet6Installed()
        {
            //check for .net 6:
            //execute dotnet --list-sdks to get versions
            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "--list-sdks";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            string standard_output;
            bool dotnet6Exists = false;

            //get command output:
            while ((standard_output = process.StandardOutput.ReadLine()) != null)
            {
                //get the major version and see if its greater than or equal to 6
                int majorVer = int.Parse(standard_output.Split(".")[0]);
                if (majorVer >= 6)
                {
                    dotnet6Exists = true;
                    break;
                }
            }
            process.WaitForExit();
            return dotnet6Exists;
        }
    }
}