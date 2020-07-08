using System;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Electron Exception
    /// </summary>
    [Serializable]
    public class ElectronException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public ElectronException()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public ElectronException(string error) : base(error)
        {

        }
    }
}
