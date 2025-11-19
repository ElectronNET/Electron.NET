namespace ElectronNET.Runtime.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal class LaunchOrderDetector
    {
        public static bool CheckIsLaunchedByDotNet()
        {
            var tests = new List<Func<bool?>>();

            tests.Add(CheckIsDotNetStartup1);
            tests.Add(CheckIsDotNetStartup2);
            tests.Add(CheckIsDotNetStartup3);

            int scoreDotNet = 0, scoreElectron = 0;

            foreach (var test in tests)
            {
                var res = test();

                if (res == true)
                {
                    scoreDotNet++;
                }

                if (res == false)
                {
                    scoreElectron++;
                }
            }

            Console.WriteLine("Probe scored for launch origin:   DotNet {0} vs. {1} Electron", scoreDotNet, scoreElectron);
            return scoreDotNet > scoreElectron;
        }

        private static bool? CheckIsDotNetStartup1()
        {
            var host = ElectronHostEnvironment.InternalHost;
            var hasPortArg = host.ProcessArguments.Any(e => e.Contains(ElectronHostDefaults.ElectronPortArgumentName, StringComparison.OrdinalIgnoreCase));
            if (hasPortArg)
            {
                return false;
            }


            return true;
        }

        private static bool? CheckIsDotNetStartup2()
        {
            var host = ElectronHostEnvironment.InternalHost;
            var hasPidArg = host.ProcessArguments.Any(e => e.Contains(ElectronHostDefaults.ElectronPidArgumentName, StringComparison.OrdinalIgnoreCase));
            if (hasPidArg)
            {
                return false;
            }

            return true;
        }

        private static bool? CheckIsDotNetStartup3()
        {
            if (Debugger.IsAttached)
            {
                return true;
            }

            return null;
        }
    }
}