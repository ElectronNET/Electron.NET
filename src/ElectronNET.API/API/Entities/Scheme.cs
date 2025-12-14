namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Authentication scheme names used by webContents 'login' authInfo.scheme.
    /// </summary>
    /// <yremarks>Undecidable from MCP: no typed enum is defined in docs; kept as project enum to reflect commonly observed values.</yremarks>
    public enum Scheme
    {
        /// <summary>
        /// 
        /// </summary>
        basic,

        /// <summary>
        /// 
        /// </summary>
        digest,

        /// <summary>
        /// 
        /// </summary>
        ntlm,

        /// <summary>
        /// 
        /// </summary>
        negotiate
    }
}