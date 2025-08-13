using SixLabors.ImageSharp.Formats.Png;

namespace ThumbnailMakers;

/// <summary>
/// Represents the output format for thumbnails.
/// </summary>
public sealed record OutputFormat
{
    /// <summary>
    /// The kind of output format, either "jpeg" or "png".
    /// </summary>
    public string Kind { get; init; } = "jpeg"; // jpeg|png
    /// <summary>
    /// The quality for JPEG output (1..100).
    /// </summary>
    public int Quality { get; init; } = 85; // jpeg only (1..100)
    /// <summary>
    /// The PNG compression level.
    /// </summary>
    public PngCompressionLevel PngCompressionLevel { get; init; } = PngCompressionLevel.DefaultCompression;

    /// <summary>
    /// Creates an OutputFormat for JPEG with the specified quality.
    /// </summary>
    /// <param name="quality">JPEG quality (1..100).</param>
    /// <returns>A new <see cref="OutputFormat"/> instance for JPEG.</returns>
    public static OutputFormat Jpeg(int quality = 85) => new() { Kind = "jpeg", Quality = quality };
    /// <summary>
    /// Creates an OutputFormat for PNG with the specified compression level.
    /// </summary>
    /// <param name="level">PNG compression level.</param>
    /// <returns>A new <see cref="OutputFormat"/> instance for PNG.</returns>
    public static OutputFormat Png(PngCompressionLevel level = PngCompressionLevel.DefaultCompression) => new() { Kind = "png", PngCompressionLevel = level };
}
