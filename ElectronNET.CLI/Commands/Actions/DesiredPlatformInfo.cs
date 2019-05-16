﻿using System;
using System.Runtime.InteropServices;

namespace ElectronNET.CLI.Commands.Actions {

    /// <summary> List of platforms we can pick from. </summary>
    public enum DesiredPlatformInfo {

        /// <summary> Windows based platform. </summary>
        win,

        /// <summary> Apple osx based platform. </summary>
        osx,

        /// <summary> Linux platform. </summary>
        linux,

        /// <summary> Linux arm platform. </summary>
        linux_arm,

        /// <summary> Attempt to auto detect the platform values. </summary>
        auto_detect,

        /// <summary> Other / custom platform </summary>
        other,

    }

    /// <summary> Extension methods for DesiredPlatformInfo. </summary>
    public static class DesiredPlatformInfoExtensions {

        /// <summary> Determine the .net core publish rid based on the enum. </summary>
        /// <param name="type"> The type to act on. </param>
        /// <returns> .net core publish rid. </returns>
        public static string ToNetCorePublishRid(this DesiredPlatformInfo type) {
            switch (type) {
                case DesiredPlatformInfo.win:
                    return "win-x64";
                case DesiredPlatformInfo.osx:
                    return "osx-x64";
                case DesiredPlatformInfo.linux:
                    return "linux-x64";
                case DesiredPlatformInfo.linux_arm:
                    return "linux-arm";
                case DesiredPlatformInfo.auto_detect: {

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        return $"win-x{(Environment.Is64BitOperatingSystem ? "64" : "86")}";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        return "osx-x64";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        return "linux-x64";
                    return null;
                }
                case DesiredPlatformInfo.other:
                    return null;
                default:
                    return null;
            }
        }

        /// <summary> Determine the electron packer platform based on the enum. </summary>
        /// <param name="type"> The type to act on. </param>
        /// <returns> electron packer platform. </returns>
        public static string ToElectronPackerPlatform(this DesiredPlatformInfo type) {
            switch (type) {
                case DesiredPlatformInfo.win:
                    return "win32";
                case DesiredPlatformInfo.osx:
                    return "darwin";
                case DesiredPlatformInfo.linux:
                    return "linux";
                case DesiredPlatformInfo.linux_arm:
                    return "linux";
                case DesiredPlatformInfo.auto_detect: {

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        return "win32";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        return "darwin";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        return "linux";
                    return null;
                }
                case DesiredPlatformInfo.other:
                    return null;
                default:
                    return null;
            }
        }
    }
}