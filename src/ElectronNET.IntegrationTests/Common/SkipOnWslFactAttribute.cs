namespace ElectronNET.IntegrationTests.Common
{
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class SkipOnWslFactAttribute : FactAttribute
    {
        private static readonly bool IsOnWsl;

        static SkipOnWslFactAttribute()
        {
            IsOnWsl = DetectWsl();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkipOnWslFactAttribute" /> class.
        /// </summary>
        public SkipOnWslFactAttribute()
        {
            if (IsOnWsl)
            {
                this.Skip = "Skipping test on WSL environment.";
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
    }
}
