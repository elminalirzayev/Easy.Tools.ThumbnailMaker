namespace ThumbnailMakers;

/// <summary>
/// Defines how the thumbnail should be resized to fit within the specified dimensions.
/// </summary>
public enum ThumbnailResizeMode
{
    /// <summary>
    /// Fit within WxH (preserve aspect).
    /// </summary>
    Fit,

    /// <summary>
    /// Fill WxH (may crop).
    /// </summary>
    Cover,

    /// <summary>
    /// Scale down only (no upscale).
    /// </summary>
    Contain,

    /// <summary>
    /// Fit then pad to WxH with background.
    /// </summary>
    Pad,

    /// <summary>
    /// Center crop to WxH (no scale if not required).
    /// </summary>
    Crop
}
