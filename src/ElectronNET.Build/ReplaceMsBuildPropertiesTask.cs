namespace ElectronNET.Build
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class ReplaceTemplateTask : Task
    {
        [Required]
        public string TemplateFile { get; set; }

        [Required]
        public string OutputFile { get; set; }

        [Required]
        public ITaskItem[] TemplateProperties { get; set; }

        public override bool Execute()
        {
            try
            {
                ////var props = this.BuildEngine9.GetGlobalProperties();

                ////var globalProperties = props
                ////    .Select(e => string.Format("{0}: {1}", e.Key, e.Value));

                ////this.Log.LogMessage(MessageImportance.High, "Global Properties: \r\n" + string.Join(Environment.NewLine, globalProperties));

                ////var envVariables = Environment.GetEnvironmentVariables();
                ////var envList = new List<string>();
                ////foreach (var v in envVariables.Keys)
                ////{
                ////    envList.Add(string.Format("{0}: {1}", v, envVariables[v]));
                ////}

                ////this.Log.LogMessage(MessageImportance.High, "Environment Variables: \r\n" + string.Join(Environment.NewLine, envList));

                string content = File.ReadAllText(this.TemplateFile);

                // Build a dictionary of property names and values.
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in this.TemplateProperties)
                {
                    dict[item.ItemSpec] = item.GetMetadata("Value").Replace("\\", "\\\\");
                }

                // Regex pattern to match placeholders like $(PropertyName)
                string pattern = @"\$\((?<prop>\w+)\)";
                content = Regex.Replace(content, pattern, match =>
                {
                    string propName = match.Groups["prop"].Value;
                    return dict.TryGetValue(propName, out var value) ? value : match.Value;
                });

                // Check if the output file exists and read its content
                if (File.Exists(this.OutputFile))
                {
                    string existingContent = File.ReadAllText(this.OutputFile);
                    // Only write the file if the content has changed
                    if (existingContent != content)
                    {
                        File.WriteAllText(this.OutputFile, content);
                    }
                }
                else
                {
                    // Write the transformed content to the output file if it doesn't exist
                    File.WriteAllText(this.OutputFile, content);
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