using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ElectronNET.CLI.Commands
{
    public class VersionCommand : ICommand
    {
        public const string COMMAND_NAME = "version";
        public const string COMMAND_DESCRIPTION = "Displays the ElectronNET.CLI version";
        public const string COMMAND_ARGUMENTS = "";
        public static IList<CommandOption> CommandOptions { get; set; } = new List<CommandOption>();

        public VersionCommand(string[] args)
        {
        }

        public Task<bool> ExecuteAsync()
        {
            return Task.Run(() =>
            {
                var runtimeVersion = typeof(VersionCommand)
                    .GetTypeInfo()
                    .Assembly
                    .GetCustomAttribute<AssemblyFileVersionAttribute>();

                Console.WriteLine($"ElectronNET.CLI Version: " + runtimeVersion.Version);

                return true;
            });
        }

    }
}