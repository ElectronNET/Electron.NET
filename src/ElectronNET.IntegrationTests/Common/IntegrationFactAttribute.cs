namespace ElectronNET.IntegrationTests.Common
{
    using System.Runtime.InteropServices;
    using Xunit.Sdk;

    /// <summary>
    /// Custom fact attribute with a default timeout of 20 seconds, allowing tests to be skipped on specific environments.
    /// </summary>
    /// <seealso cref="Xunit.FactAttribute" />
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("Xunit.Sdk.SkippableFactDiscoverer", "Xunit.SkippableFact")]
    internal sealed class IntegrationFactAttribute : FactAttribute
    {
        private static readonly bool IsOnWsl;

        private static readonly bool IsOnCI;

        static IntegrationFactAttribute()
        {
            IsOnWsl = DetectWsl();
            IsOnCI = DetectCI();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationFactAttribute" /> class.
        /// </summary>
        public IntegrationFactAttribute()
        {
            this.Timeout = 20_000;
        }

        public bool SkipOnWsl { get; set; }

        public bool SkipOnCI { get; set; }

        /// <summary>
        /// Marks the test so that it will not be run, and gets or sets the skip reason
        /// </summary>
        public override string Skip {
            get
            {
                if (IsOnWsl && this.SkipOnWsl)
                {
                    return "Skipping test on WSL environment.";
                }

                if (IsOnCI && this.SkipOnCI)
                {
                    return "Skipping test on CI environment.";
                }

                return base.Skip;
            }
            set
            {
                base.Skip = value;
            }
        }

        private static bool DetectWsl()
        {
            try
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WSL_DISTRO_NAME")) ||
                    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WSL_INTEROP")))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private static bool DetectCI()
        {
            try
            {
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")) ||
                    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
