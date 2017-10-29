using ElectronNET.CLI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ElectronNET.CLI
{
 
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsageHeader();
                PrintUsage();
                Environment.Exit(-1);
            }

            ICommand command = null;

            switch (args[0])
            {
                case StartElectronCommand.COMMAND_NAME:
                    command = new StartElectronCommand(args.Skip(1).ToArray());
                    break;
                case BuildCommand.COMMAND_NAME:
                    command = new BuildCommand(args.Skip(1).ToArray());
                    break;
                case InitCommand.COMMAND_NAME:
                    command = new InitCommand(args.Skip(1).ToArray());
                    break;
                case VersionCommand.COMMAND_NAME:
                    command = new VersionCommand(args.Skip(1).ToArray());
                    break;
                case "--help":
                case "--h":
                case "help":
                    PrintUsageHeader();

                    if (args.Length > 1)
                        PrintUsage(args[1]);
                    else
                        PrintUsage();
                    break;
                default:
                    Console.Error.WriteLine($"Unknown command {args[0]}");
                    PrintUsage();
                    Environment.Exit(-1);
                    break;
            }

            if (command != null)
            {
                var success = command.ExecuteAsync().Result;
                if (!success)
                {
                    Environment.Exit(-1);
                }
            }
        }

        private static void PrintUsageHeader()
        {
            var sb = new StringBuilder("Electron.NET Tools");
            var version = Version;
            if (!string.IsNullOrEmpty(version))
            {
                sb.Append($" ({version})");
            }
            Console.WriteLine(sb.ToString());
            Console.WriteLine("Project Home: https://github.com/GregorBiswanger/ElectronNET");
            Console.WriteLine("\t");
        }

        private static void PrintUsage()
        {
            const int NAME_WIDTH = 23;
            Console.WriteLine("\t");
            Console.WriteLine("Commands to start the Electron Application:");
            Console.WriteLine("\t");
            Console.WriteLine($"\t{StartElectronCommand.COMMAND_NAME.PadRight(NAME_WIDTH)} {StartElectronCommand.COMMAND_DESCRIPTION}");

            Console.WriteLine("\t");
            Console.WriteLine("Commands to build the Electron Application:");
            Console.WriteLine("\t");
            Console.WriteLine($"\t{BuildCommand.COMMAND_NAME.PadRight(NAME_WIDTH)} {BuildCommand.COMMAND_DESCRIPTION}");

            Console.WriteLine("\t");
            Console.WriteLine("Commands to init the Electron Application:");
            Console.WriteLine("\t");
            Console.WriteLine($"\t{InitCommand.COMMAND_NAME.PadRight(NAME_WIDTH)} {InitCommand.COMMAND_DESCRIPTION}");

            Console.WriteLine("\t");
            Console.WriteLine("Commands to see the current ElectronNET version number:");
            Console.WriteLine("\t");
            Console.WriteLine($"\t{VersionCommand.COMMAND_NAME.PadRight(NAME_WIDTH)} {VersionCommand.COMMAND_DESCRIPTION}");

            Console.WriteLine("\t");
            Console.WriteLine("\t");
            Console.WriteLine("To get help on individual commands execute:");
            Console.WriteLine("\tdotnet electronize help <command>");
        }

        private static void PrintUsage(string command)
        {
            switch (command)
            {
                case StartElectronCommand.COMMAND_NAME:
                    PrintUsage(StartElectronCommand.COMMAND_NAME, StartElectronCommand.COMMAND_DESCRIPTION, StartElectronCommand.CommandOptions, StartElectronCommand.COMMAND_ARGUMENTS);
                    break;
                case BuildCommand.COMMAND_NAME:
                    PrintUsage(BuildCommand.COMMAND_NAME, BuildCommand.COMMAND_DESCRIPTION, BuildCommand.CommandOptions, BuildCommand.COMMAND_ARGUMENTS);
                    break;
                case InitCommand.COMMAND_NAME:
                    PrintUsage(InitCommand.COMMAND_NAME, InitCommand.COMMAND_DESCRIPTION, InitCommand.CommandOptions, InitCommand.COMMAND_ARGUMENTS);
                    break;
                case VersionCommand.COMMAND_NAME:
                    PrintUsage(VersionCommand.COMMAND_NAME, VersionCommand.COMMAND_DESCRIPTION, VersionCommand.CommandOptions, VersionCommand.COMMAND_ARGUMENTS);
                    break;
                default:
                    Console.Error.WriteLine($"Unknown command {command}");
                    PrintUsage();
                    break;
            }
        }

        private static void PrintUsage(string command, string description, IList<CommandOption> options, string arguments)
        {
            const int INDENT = 3;

            Console.WriteLine($"{command}: ");
            Console.WriteLine($"{new string(' ', INDENT)}{description}");
            Console.WriteLine("\t");


            if (!string.IsNullOrEmpty(arguments))
            {
                Console.WriteLine($"{new string(' ', INDENT)}dotnet electronize {command} [arguments] [options]");
                Console.WriteLine($"{new string(' ', INDENT)}Arguments:");
                Console.WriteLine($"{new string(' ', INDENT * 2)}{arguments}");
            }
            else
            {
                Console.WriteLine($"{new string(' ', INDENT)}dotnet electronize {command} [options]");
            }

            const int SWITCH_COLUMN_WIDTH = 40;

            Console.WriteLine($"{new string(' ', INDENT)}Options:");
            foreach (var option in options)
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (option.ShortSwitch != null)
                {
                    stringBuilder.Append($"{option.ShortSwitch.PadRight(6)} | ");
                }

                stringBuilder.Append($"{option.Switch}");
                if (stringBuilder.Length < SWITCH_COLUMN_WIDTH)
                {
                    stringBuilder.Append(new string(' ', SWITCH_COLUMN_WIDTH - stringBuilder.Length));
                }

                stringBuilder.Append(option.Description);


                Console.WriteLine($"{new string(' ', INDENT * 2)}{stringBuilder.ToString()}");
            }
        }

        private static string Version
        {
            get
            {
                AssemblyInformationalVersionAttribute attribute = null;
                try
                {
                    attribute = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                }
                catch (AmbiguousMatchException)
                {
                    // Catch exception and continue if multiple attributes are found.
                }
                return attribute?.InformationalVersion;
            }
        }
    }
}
