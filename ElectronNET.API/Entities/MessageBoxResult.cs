namespace ElectronNET.API.Entities
{
    internal class MessageBoxResponse
    {
        public int response { get; set; }
        public bool @checked { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageBoxResult
    {
        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public int Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [checkbox checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [checkbox checked]; otherwise, <c>false</c>.
        /// </value>
        public bool CheckboxChecked { get; set; }
    }
}
