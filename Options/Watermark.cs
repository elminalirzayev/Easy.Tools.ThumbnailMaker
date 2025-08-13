using SixLabors.ImageSharp.PixelFormats;

namespace ThumbnailMakers;

/// <summary>
/// Represents a watermark to be applied to thumbnails.
/// </summary>
public sealed class Watermark
{
    /// <summary>
    /// The watermark text.
    /// </summary>
    public string Text { get; init; } = string.Empty;
    /// <summary>
    /// The font size of the watermark text.
    /// </summary>
    public float FontSize { get; init; } = 16f;
    /// <summary>
    /// The opacity of the watermark text (0 to 1).
    /// </summary>
    public float Opacity { get; init; } = 0.35f;
    /// <summary>
    /// The anchor position of the watermark.
    /// </summary>
    public Anchor Position { get; init; } = Anchor.BottomRight;
    /// <summary>
    /// The margin in pixels from the anchor position.
    /// </summary>
    public int Margin { get; init; } = 8;
    /// <summary>
    /// The font family name for the watermark text.
    /// </summary>
    public string FontFamily { get; init; } = "Arial";
    /// <summary>
    /// The color of the watermark text.
    /// </summary>
    public ThumbnailColor Color { get; init; } = new ThumbnailColor(255, 255, 255, 255); // default White 

    /// <summary>
    /// Gets a watermark instance representing no watermark.
    /// </summary>
    public static Watermark None => new Watermark { Text = string.Empty, Opacity = 0f };

    /// <summary>
    /// Creates a text watermark with the specified options.
    /// </summary>
    /// <param name="text">The watermark text.</param>
    /// <param name="size">The font size.</param>
    /// <param name="opacity">The opacity (0 to 1).</param>
    /// <param name="position">The anchor position.</param>
    /// <param name="margin">The margin in pixels.</param>
    /// <param name="fontFamily">The font family name.</param>
    /// <param name="color">The color of the watermark text.</param>
    /// <returns>A new <see cref="Watermark"/> instance.</returns>
    public static Watermark TextMark(
        string text,
        float size = 16f,
        float opacity = 0.35f,
        Anchor position = Anchor.BottomRight,
        int margin = 8,
        string fontFamily = "Arial",
        ThumbnailColor? color = null
    )
    {
        if (opacity < 0f || opacity > 1f)
            throw new ArgumentOutOfRangeException(nameof(opacity), "Opacity must be between 0 and 1.");
        return new Watermark
        {
            Text = text,
            FontSize = size,
            Opacity = opacity,
            Position = position,
            Margin = margin,
            FontFamily = fontFamily,
            Color = color ?? new ThumbnailColor(255, 255, 255, (byte)(opacity * 255))
        };
    }
}

