using SixLabors.ImageSharp.PixelFormats;

namespace ThumbnailMakers;

/// <summary>
/// Represents options for generating thumbnails.
/// </summary>
public sealed record ThumbnailOptions
{
    /// <summary>
    /// The width of the thumbnail.
    /// </summary>
    public int Width { get; init; }
    /// <summary>
    /// The height of the thumbnail.
    /// </summary>
    public int Height { get; init; }
    /// <summary>
    /// The resize mode for the thumbnail.
    /// </summary>
    public ThumbnailResizeMode Mode { get; init; } = ThumbnailResizeMode.Fit;
    /// <summary>
    /// The anchor point for the thumbnail.
    /// </summary>
    public Anchor Anchor { get; init; } = Anchor.Center;
    /// <summary>
    /// Whether to auto-orient the image based on EXIF data.
    /// </summary>
    public bool AutoOrient { get; init; } = true;
    /// <summary>
    /// Whether to strip metadata from the image.
    /// </summary>
    public bool StripMetadata { get; init; } = true;
    /// <summary>
    /// The background color for padding (if applicable).
    /// </summary>
    public ColorBackground? Background { get; init; } = null; // null => no pad unless Mode.Pad
    /// <summary>
    /// The output format for the thumbnail.
    /// </summary>
    public OutputFormat Output { get; init; } = OutputFormat.Jpeg();
    /// <summary>
    /// The watermark to apply to the thumbnail.
    /// </summary>
    public Watermark Watermark { get; init; } = Watermark.None;
    /// <summary>
    /// Whether to prevent upscaling the image.
    /// </summary>
    public bool PreventUpscale { get; init; } = false;

    /// <summary>
    /// Initializes a new instance of <see cref="ThumbnailOptions"/>.
    /// </summary>
    /// <param name="width">The width of the thumbnail.</param>
    /// <param name="height">The height of the thumbnail.</param>
    public ThumbnailOptions(int width, int height) => (Width, Height) = (width, height);
}
