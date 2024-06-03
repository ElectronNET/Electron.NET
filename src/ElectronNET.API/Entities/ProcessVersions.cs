namespace ElectronNET.API.Entities
{
    /// <summary>
    /// An object listing the version strings specific to Electron
    /// </summary>
    /// <param name="Chrome">Value representing Chrome's version string</param>
    /// <param name="Electron">Value representing Electron's version string</param>
    /// <returns></returns>
    public record ProcessVersions(string Chrome, string Electron);
} 