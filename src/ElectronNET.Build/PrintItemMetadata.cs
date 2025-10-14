namespace ElectronNET.Build
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class DumpItemMetadataTask : Task
    {
        // The item group whose metadata will be dumped.
        [Required]
        public ITaskItem[] Items { get; set; }

        public override bool Execute()
        {
            try
            {
                foreach (var item in this.Items)
                {
                    // Log the item's identity (the Include attribute)
                    this.Log.LogMessage(MessageImportance.High, $"Item: {item.ItemSpec}");

                    // Iterate through each metadata field of the item.
                    foreach (string metadataName in item.MetadataNames)
                    {
                        string metadataValue = item.GetMetadata(metadataName);
                        this.Log.LogMessage(MessageImportance.High, $"    {metadataName}: {metadataValue}");
                    }
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