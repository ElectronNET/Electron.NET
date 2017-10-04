using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace ElectronNET.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.Name = "electronnet";
            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                Console.WriteLine("Hello World!");
                return 0;
            });

            app.Command("start", (command) => {
                command.Description = "Start ASP.NET Core project with Electron.";
                command.HelpOption("-?|-h|--help");

                command.Argument("path", "Path to the ASP.NET Core project");

                command.OnExecute(() => {
                    Console.WriteLine(Directory.GetCurrentDirectory());
                    Console.WriteLine(command.Arguments[0].Value);
                });

            });

            return app.Execute(args);
        }
    }
}
