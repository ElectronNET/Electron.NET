namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Opens the devtools with specified dock state, can be right, bottom, undocked,
    /// detach.Defaults to last used dock state.In undocked mode it's possible to dock
    /// back.In detach mode it's not.
    /// </summary>
    public enum DevToolsMode
    {
        right,
        bottom,
        undocked,
        detach
    }
}