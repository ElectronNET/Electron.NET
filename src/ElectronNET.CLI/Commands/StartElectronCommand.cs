using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ElectronNET.CLI.Commands.Actions;

namespace ElectronNET.CLI.Commands;

public class StartElectronCommand : ICommand
{
    public const string COMMAND_NAME = "start";
    public const string COMMAND_DESCRIPTION = "Start your ASP.NET Core Application with Electron, without package it as a single exe. Faster for development.";
    public const string COMMAND_ARGUMENTS = "<Path> from ASP.NET Core Project.";
    public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

    private static SimpleCommandLineParser _parser = new();

    public StartElectronCommand(string[] args) => _parser.Parse(args);

    private string _aspCoreProjectPath = "project-path";
    private string _arguments = "args";
    private string _manifest = "manifest";
    private string _clearCache = "clear-cache";
    private string _paramPublishReadyToRun = "PublishReadyToRun";
    private string _paramPublishSingleFile = "PublishSingleFile";
    private string _paramDotNetConfig = "dotnet-configuration";
    private string _paramTarget = "target";

    public Task<bool> ExecuteAsync()
    {
        return Task.Run(() =>
        {
            Console.WriteLine("Start Electron Desktop Application...");

            var aspCoreProjectPath =
                _parser.Arguments.TryGetValue(_aspCoreProjectPath, out var projectPathData) &&
                Directory.Exists(projectPathData.First())
                    ? projectPathData.First()
                    : Directory.GetCurrentDirectory();

            var tempPath = Path.Combine(aspCoreProjectPath, "obj", "Host");
            if (Directory.Exists(tempPath) == false)
                Directory.CreateDirectory(tempPath);

            var tempBinPath = Path.Combine(tempPath, "bin");
            var resultCode = 0;

            var publishReadyToRun = _parser.Arguments.TryGetValue(_paramPublishReadyToRun, out var readyToRunData)
                ? $"/p:{_paramPublishReadyToRun}={readyToRunData[0]}"
                : $"/p:{_paramPublishReadyToRun}=true";

            var publishSingleFile = _parser.Arguments.TryGetValue(_paramPublishSingleFile, out var singleFileData)
                ? $"/p:{_paramPublishSingleFile}={singleFileData[0]}"
                : $"/p:{_paramPublishSingleFile}=true";;

            // If target is specified as a command line argument, use it.
            // Format is the same as the build command.
            // If target is not specified, autodetect it.
            var platformInfo = GetTargetPlatformInformation.Do(string.Empty, string.Empty);
            if (_parser.Arguments.ContainsKey(_paramTarget))
            {
                var desiredPlatform = _parser.Arguments[_paramTarget][0];
                var specifiedFromCustom = desiredPlatform == "custom" && _parser.Arguments[_paramTarget].Length > 1
                    ? _parser.Arguments[_paramTarget][1]
                    : string.Empty;
                platformInfo = GetTargetPlatformInformation.Do(desiredPlatform, specifiedFromCustom);
            }

            var configuration = _parser.Arguments.TryGetValue(_paramDotNetConfig, out var configData) ? configData[0] : "Debug";

            if (_parser != null && !_parser.Arguments.ContainsKey("watch"))
            {
                resultCode = ProcessHelper.CmdExecute($"dotnet publish -r {platformInfo.NetCorePublishRid} -c \"{configuration}\" --output \"{tempBinPath}\" {publishReadyToRun} {publishSingleFile} --no-self-contained", aspCoreProjectPath);
            }

            if (resultCode != 0)
            {
                Console.WriteLine($"Error occurred during dotnet publish: {resultCode}");
                return false;
            }

            Console.WriteLine("ElectronHostHook handling started...");

            var hostHookFolders = Directory.GetDirectories(aspCoreProjectPath, "ElectronHostHook", SearchOption.AllDirectories);

            DeployEmbeddedElectronFiles.Do(tempPath, hostHookFolders.Length > 0);

            if (hostHookFolders.Length > 0)
            {
                var hostHookDir = Path.Combine(tempPath, "ElectronHostHook");
                DirectoryCopy.Do(hostHookFolders.First(), hostHookDir, true, new List<string>() { "node_modules" });
            }

            Console.WriteLine("Start npm install...");
            ProcessHelper.CmdExecute("npm install", tempPath);

            var arguments = new List<string>();;

            if (_parser.Arguments.TryGetValue(_arguments, out var argumentData))
                arguments.Add(string.Join(' ', argumentData));

            if (_parser.Arguments.TryGetValue(_manifest, out var manifestData))
                arguments.Add("--manifest=" + manifestData.First());

            if (_parser.Arguments.ContainsKey(_clearCache))
                arguments.Add("--clear-cache=true");

            if (_parser.Arguments.ContainsKey("watch"))
                arguments.Add("--watch=true");

            var path = Path.Combine(tempPath, "node_modules", ".bin");
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                Console.WriteLine("Invoke electron.cmd - in dir: " + path);
                ProcessHelper.CmdExecute(@"electron.cmd ""..\..\main.js"" " + string.Join(' ', arguments), path);
            }
            else
            {
                Console.WriteLine("Invoke electron - in dir: " + path);
                ProcessHelper.CmdExecute(@"./electron ""../../main.js"" " + string.Join(' ', arguments), path);
            }

            return true;
        });
    }
}