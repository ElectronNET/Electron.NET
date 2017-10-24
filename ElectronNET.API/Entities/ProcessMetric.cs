namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessMetric
    {
      /// <summary>
      /// CPU usage of the process.
      /// </summary>
      public CPUUsage Cpu { get; set; }

      /// <summary>
      /// Memory information for the process.
      /// </summary>
      public MemoryInfo Memory {get; set;}

      /// <summary>
      /// Process id of the process.
      /// </summary>
      public int Pid { get; set; }

      /// <summary>
      /// Process type (Browser or Tab or GPU etc).
      /// </summary>
      public string Type { get; set; }
    }
}
