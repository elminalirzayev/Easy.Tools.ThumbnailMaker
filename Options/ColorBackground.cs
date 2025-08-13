namespace ThumbnailMakers;

/// <summary>
/// Represents a color background for thumbnails.
/// </summary>
/// <param name="R">Red channel (0-255).</param>
/// <param name="G">Green channel (0-255).</param>
/// <param name="B">Blue channel (0-255).</param>
/// <param name="A">Alpha channel (0-255, default 255).</param>
public sealed record ColorBackground(byte R, byte G, byte B, byte A = 255)
{
    /// <summary>
    /// Gets a transparent background (RGBA: 0,0,0,0).
    /// </summary>
    public static ColorBackground Transparent => new(0, 0, 0, 0);
    /// <summary>
    /// Gets a white background (RGBA: 255,255,255,255).
    /// </summary>
    public static ColorBackground White => new(255, 255, 255, 255);
    /// <summary>
    /// Gets a black background (RGBA: 0,0,0,255).
    /// </summary>
    public static ColorBackground Black => new(0, 0, 0, 255);
}
