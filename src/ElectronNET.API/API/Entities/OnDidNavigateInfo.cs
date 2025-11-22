namespace ElectronNET.API.Entities;

/// <summary>
/// 'did-navigate' event details for main frame navigation.
/// </summary>
/// <remarks>Up-to-date with Electron API 39.2</remarks>
public class OnDidNavigateInfo
{
    /// <summary>
    /// The URL navigated to.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// HTTP response code (-1 for non-HTTP navigations).
    /// </summary>
    public int HttpResponseCode { get; set; }

    /// <summary>
    /// HTTP status text (empty for non-HTTP navigations).
    /// </summary>
    public string HttpStatusText { get; set; }
}