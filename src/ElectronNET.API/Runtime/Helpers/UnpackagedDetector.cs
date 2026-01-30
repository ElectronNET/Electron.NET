namespace ElectronNET.Runtime.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    internal class UnpackagedDetector
    {
        // Cache debugger state to avoid multiple expensive checks
        private static bool? _debuggerAttached;
        
        public static bool CheckIsUnpackaged()
        {
            var tests = new List<Func<bool?>>();

            tests.Add(CheckUnpackaged1);

            // We let this one account twice
            tests.Add(CheckUnpackaged2);
            tests.Add(CheckUnpackaged2);

            tests.Add(CheckUnpackaged3);
            tests.Add(CheckUnpackaged4);

            int scoreUnpackaged = 0, scorePackaged = 0;

            foreach (var test in tests)
            {
                var res = test();

                if (res == true)
                {
                    scoreUnpackaged++;
                }

                if (res == false)
                {
                    scorePackaged++;
                }
            }

            Console.WriteLine("Probe scored for package mode:   Unpackaged {0} vs. {1} Packaged", scoreUnpackaged, scorePackaged);
            return scoreUnpackaged > scorePackaged;
        }

        private static bool? CheckUnpackaged1()
        {
            var cfg = ElectronNetRuntime.BuildInfo.BuildConfiguration;
            if (cfg != null)
            {
                if (cfg.Equals("Debug", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (cfg.Equals("Release", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return null;
        }

        private static bool? CheckUnpackaged2()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dir = new DirectoryInfo(baseDir);
            
            if (dir.Name == "bin" && dir.Parent?.Name == "resources")
            {
                return false;
            }

            // Faster: Direct path check instead of directory enumeration
            if (Directory.Exists(Path.Combine(baseDir, ".electron")))
            {
                return true;
            }

            return null;
        }

        private static bool? CheckUnpackaged3()
        {
            // Cache debugger state to avoid multiple expensive checks
            if (_debuggerAttached == null)
            {
                _debuggerAttached = Debugger.IsAttached;
            }
            
            if (_debuggerAttached.Value)
            {
                return true;
            }

            return null;
        }

        private static bool? CheckUnpackaged4()
        {
            var isUnpackaged = ElectronNetRuntime.ProcessArguments.Any(e => e.Contains("unpacked", StringComparison.OrdinalIgnoreCase));

            if (isUnpackaged)
            {
                return true;
            }

            var isPackaged = ElectronNetRuntime.ProcessArguments.Any(e => e.Contains("dotnetpacked", StringComparison.OrdinalIgnoreCase));
            if (isPackaged)
            {
                return false;
            }

            return null;
        }
    }
}