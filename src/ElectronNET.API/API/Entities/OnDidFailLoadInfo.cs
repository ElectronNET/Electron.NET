namespace ElectronNET.API.Entities;

/// <summary>
/// 'OnDidFailLoad' event details.
/// </summary>
public class OnDidFailLoadInfo
{
    /// <summary>
    /// The full list of error codes and their meaning is available here
    /// https://source.chromium.org/chromium/chromium/src/+/main:net/base/net_error_list.h
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// Validated URL.
    /// </summary>
    public string ValidatedUrl { get; set; }
}