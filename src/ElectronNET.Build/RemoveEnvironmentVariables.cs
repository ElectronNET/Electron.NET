namespace ElectronNET.Build
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class RemoveEnvironmentVariables : Task
    {
        [Required]
        public string Variables { get; set; }

        public override bool Execute()
        {
            try
            {
                if (string.IsNullOrEmpty(this.Variables))
                {
                    this.Log.LogError("The Variables property is not set");
                    return false;
                }

                var items = this.Variables.Split(new[] { ':', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in items)
                {
                    Environment.SetEnvironmentVariable(item.Trim(), null);
                    this.Log.LogMessage("Unset environment variable: {0}", item);
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}