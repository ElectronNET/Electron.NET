using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ElectronNET.API
{
    public static partial class Electron
    {
        /// <summary>
        /// Experimental code, use with care
        /// </summary>
        public static class Experimental
        {
            /// <summary>
            /// Starts electron from C#, use during development to avoid having to fully publish / build your app on every compile cycle
            /// You will need to run the CLI at least once (and once per update) to bootstrap all required files
            /// </summary>
            /// <param name="webPort"></param>
            /// <param name="projectPath"></param>
            /// <param name="extraElectronArguments"></param>
            /// <param name="clearCache"></param>
            /// <exception cref="DirectoryNotFoundException"></exception>
            /// <exception cref="Exception"></exception>
            public static async Task<int> StartElectronForDevelopment(int webPort, string projectPath = null, string[] extraElectronArguments = null, bool clearCache = false)
            {
                string aspCoreProjectPath;

                if (!string.IsNullOrEmpty(projectPath))
                {
                    if (Directory.Exists(projectPath))
                    {
                        aspCoreProjectPath = projectPath;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException(projectPath);
                    }
                }
                else
                {
                    aspCoreProjectPath = Directory.GetCurrentDirectory();
                }

                string tempPath = Path.Combine(aspCoreProjectPath, "obj", "Host");

                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                var mainFileJs = Path.Combine(tempPath, "main.js");
                if (!File.Exists(mainFileJs))
                {
                    throw new Exception("You need to run once the electronize-h5 start command to bootstrap the necessary files");
                }

                var nodeModulesDirPath = Path.Combine(tempPath, "node_modules");

                bool runNpmInstall = false;

                if (!Directory.Exists(nodeModulesDirPath))
                {
                    runNpmInstall = true;
                }

                var packagesJson = Path.Combine(tempPath, "package.json");

                var packagesPrevious = Path.Combine(tempPath, "package.json.previous");

                if (!runNpmInstall)
                {
                    if (File.Exists(packagesPrevious))
                    {
                        if (File.ReadAllText(packagesPrevious) != File.ReadAllText(packagesJson))
                        {
                            runNpmInstall = true;
                        }
                    }
                    else
                    {
                        runNpmInstall = true;
                    }
                }

                if (runNpmInstall)
                {
                    throw new Exception("You need to run once the electronize-h5 start command to bootstrap the necessary files");
                }

                string arguments = "";

                if (extraElectronArguments is object)
                {
                    arguments = string.Join(' ', extraElectronArguments);
                }

                if (clearCache)
                {
                    arguments += " --clear-cache=true";
                }

                BridgeConnector.AuthKey = Guid.NewGuid().ToString().Replace("-", "");

                var socketPort = FreeTcpPort();

                arguments += $" --development=true --devauth={BridgeConnector.AuthKey} --devport={socketPort}";

                string path = Path.Combine(tempPath, "node_modules", ".bin");
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

                if (isWindows)
                {
                    ProcessHelper.Execute(@"electron.cmd ""..\..\main.js"" " + arguments, path);
                }
                else
                {
                    ProcessHelper.Execute(@"./electron ""../../main.js"" " + arguments, path);
                }

                BridgeSettings.InitializePorts(socketPort, webPort);
                await Task.Delay(500);
                
                return socketPort;
            }

            /// <summary>
            /// Return a free local TCP port
            /// </summary>
            /// <returns></returns>
            public static int FreeTcpPort()
            {
                TcpListener l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                int port = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
                return port;
            }
        }
    }
}