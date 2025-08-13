using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;
using Size = SixLabors.ImageSharp.Size;

namespace ThumbnailMakers;

/// <summary>
/// Provides static methods for generating thumbnails from images.
/// </summary>
public static class Thumbnail
{
    /// <summary>
    /// Creates a thumbnail from the input stream and returns the result as a byte array.
    /// </summary>
    /// <param name="input">The input image stream.</param>
    /// <param name="options">Thumbnail creation options.</param>
    /// <param name="ct">Cancellation token (optional).</param>
    /// <returns>Byte array of the created thumbnail image.</returns>
    public static async Task<byte[]> MakeAsync(Stream input, ThumbnailOptions options, CancellationToken ct = default)
    {
        using var output = new MemoryStream();
        await MakeAsync(input, output, options, ct);
        return output.ToArray();
    }

    /// <summary>
    /// Creates a thumbnail from the input stream and writes the result to the output stream.
    /// </summary>
    /// <param name="input">The input image stream.</param>
    /// <param name="output">The output image stream.</param>
    /// <param name="options">Thumbnail creation options.</param>
    /// <param name="ct">Cancellation token (optional).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task MakeAsync(Stream input, Stream output, ThumbnailOptions options, CancellationToken ct = default)
    {
        if (options.Width <= 0 || options.Height <= 0)
            throw new ArgumentOutOfRangeException("Width/Height must be > 0");

        using var image = await Image.LoadAsync(input, ct);

        if (options.AutoOrient)
        {
            TryAutoOrient(image);
        }

        if (options.StripMetadata)
        {
            image.Metadata.ExifProfile = null;
            image.Metadata.IccProfile = null;
            image.Metadata.XmpProfile = null;
        }

        var (destW, destH) = ComputeTargetSize(image.Width, image.Height, options);

        var resizeOptions = new ResizeOptions
        {
            Size = new Size(destW, destH),
            Sampler = KnownResamplers.Lanczos3,
            Position = MapAnchor(options.Anchor),
            Mode = MapMode(options, image.Width, image.Height, destW, destH)
        };

        image.Mutate(ctx => ctx.Resize(resizeOptions));

        if (options.Mode == ThumbnailResizeMode.Pad || options.Background is not null)
        {
            var bg = options.Background ?? ColorBackground.White;
            using var canvas = new Image<Rgba32>(options.Width, options.Height, new Rgba32(bg.R, bg.G, bg.B, bg.A));
            var location = GetPadLocation(options.Anchor, options.Width, options.Height, image.Width, image.Height);
            canvas.Mutate(c => c.DrawImage(image, new Point(location.x, location.y), 1f));
            image.Dispose();
            await SaveAsync(canvas, output, options, ct);
            return;
        }

        if (!string.IsNullOrWhiteSpace(options.Watermark.Text) && options.Watermark.Opacity > 0f)
        {
            ApplyTextWatermark(image, options.Watermark);
        }

        await SaveAsync(image, output, options, ct);
    }

    /// <summary>
    /// Computes the target size for resizing the image based on options.
    /// </summary>
    /// <param name="srcW">Source image width.</param>
    /// <param name="srcH">Source image height.</param>
    /// <param name="o">Thumbnail options.</param>
    /// <returns>Tuple of target width and height.</returns>
    private static (int w, int h) ComputeTargetSize(int srcW, int srcH, ThumbnailOptions o)
    {
        double rw = (double)o.Width / srcW;
        double rh = (double)o.Height / srcH;
        double scale = o.Mode switch
        {
            ThumbnailResizeMode.Fit => Math.Min(rw, rh),
            ThumbnailResizeMode.Cover => Math.Max(rw, rh),
            ThumbnailResizeMode.Contain => Math.Min(1.0, Math.Min(rw, rh)),
            ThumbnailResizeMode.Pad => Math.Min(rw, rh),
            ThumbnailResizeMode.Crop => Math.Min(rw, rh),
            _ => Math.Min(rw, rh)
        };
        if (o.PreventUpscale) scale = Math.Min(1.0, scale);
        var w = Math.Max(1, (int)Math.Round(srcW * scale));
        var h = Math.Max(1, (int)Math.Round(srcH * scale));
        return (w, h);
    }

    /// <summary>
    /// Maps the Anchor enum to ImageSharp's AnchorPositionMode.
    /// </summary>
    /// <param name="a">Anchor value.</param>
    /// <returns>AnchorPositionMode value.</returns>
    private static AnchorPositionMode MapAnchor(Anchor a) => a switch
    {
        Anchor.TopLeft => AnchorPositionMode.TopLeft,
        Anchor.Top => AnchorPositionMode.Top,
        Anchor.TopRight => AnchorPositionMode.TopRight,
        Anchor.Left => AnchorPositionMode.Left,
        Anchor.Center => AnchorPositionMode.Center,
        Anchor.Right => AnchorPositionMode.Right,
        Anchor.BottomLeft => AnchorPositionMode.BottomLeft,
        Anchor.Bottom => AnchorPositionMode.Bottom,
        Anchor.BottomRight => AnchorPositionMode.BottomRight,
        _ => AnchorPositionMode.Center
    };

    /// <summary>
    /// Maps the ThumbnailResizeMode to ImageSharp's ResizeMode.
    /// </summary>
    /// <param name="o">Thumbnail options.</param>
    /// <param name="srcW">Source image width.</param>
    /// <param name="srcH">Source image height.</param>
    /// <param name="destW">Destination width.</param>
    /// <param name="destH">Destination height.</param>
    /// <returns>ResizeMode value.</returns>
    private static ResizeMode MapMode(ThumbnailOptions o, int srcW, int srcH, int destW, int destH) => o.Mode switch
    {
        ThumbnailResizeMode.Fit => ResizeMode.Max,
        ThumbnailResizeMode.Cover => ResizeMode.Crop,
        ThumbnailResizeMode.Contain => ResizeMode.Max,
        ThumbnailResizeMode.Pad => ResizeMode.Max,
        ThumbnailResizeMode.Crop => ResizeMode.Crop,
        _ => ResizeMode.Max
    };

    /// <summary>
    /// Gets the location for padding the image on the canvas based on anchor.
    /// </summary>
    /// <param name="anchor">Anchor value.</param>
    /// <param name="canvasW">Canvas width.</param>
    /// <param name="canvasH">Canvas height.</param>
    /// <param name="imgW">Image width.</param>
    /// <param name="imgH">Image height.</param>
    /// <returns>Tuple of x and y coordinates.</returns>
    private static (int x, int y) GetPadLocation(Anchor anchor, int canvasW, int canvasH, int imgW, int imgH)
    {
        int x = anchor switch
        {
            Anchor.TopLeft or Anchor.Left or Anchor.BottomLeft => 0,
            Anchor.Top or Anchor.Center or Anchor.Bottom => (canvasW - imgW) / 2,
            Anchor.TopRight or Anchor.Right or Anchor.BottomRight => canvasW - imgW,
            _ => (canvasW - imgW) / 2
        };
        int y = anchor switch
        {
            Anchor.TopLeft or Anchor.Top or Anchor.TopRight => 0,
            Anchor.Left or Anchor.Center or Anchor.Right => (canvasH - imgH) / 2,
            Anchor.BottomLeft or Anchor.Bottom or Anchor.BottomRight => canvasH - imgH,
            _ => (canvasH - imgH) / 2
        };
        return (x, y);
    }

    /// <summary>
    /// Attempts to auto-orient the image based on EXIF orientation.
    /// </summary>
    /// <param name="image">The image to auto-orient.</param>
    private static void TryAutoOrient(Image image)
    {
        var exif = image.Metadata.ExifProfile;
        if (exif is null) return;

        if (exif.TryGetValue(ExifTag.Orientation, out IExifValue<ushort>? orientationValue))
        {
            image.Mutate(ctx => ctx.AutoOrient());

            exif.RemoveValue(ExifTag.Orientation);
        }
    }

    /// <summary>
    /// Applies a text watermark to the image.
    /// </summary>
    /// <param name="image">The image to watermark.</param>
    /// <param name="wm">Watermark options.</param>
    private static void ApplyTextWatermark(Image image, Watermark wm)
    {
        var fontCollection = new FontCollection();
        FontFamily family;
        if (!string.IsNullOrWhiteSpace(wm.FontFamily) && SystemFonts.TryGet(wm.FontFamily, out family))
        {
        }
        else if (SystemFonts.TryGet("Arial", out family)) // 1. fallback
        {
        }
        else if (SystemFonts.TryGet("Segoe UI", out family)) // 2. fallback
        {
        }
        else
        {
            family = SystemFonts.Families.First();
        }
        var font = family.CreateFont(wm.FontSize);

        var text = wm.Text;
        var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));
        var margin = wm.Margin;

        PointF location = wm.Position switch
        {
            Anchor.TopLeft => new PointF(margin, margin),
            Anchor.Top => new PointF((image.Width - textSize.Width) / 2, margin),
            Anchor.TopRight => new PointF(image.Width - textSize.Width - margin, margin),
            Anchor.Left => new PointF(margin, (image.Height - textSize.Height) / 2),
            Anchor.Center => new PointF((image.Width - textSize.Width) / 2, (image.Height - textSize.Height) / 2),
            Anchor.Right => new PointF(image.Width - textSize.Width - margin, (image.Height - textSize.Height) / 2),
            Anchor.BottomLeft => new PointF(margin, image.Height - textSize.Height - margin),
            Anchor.Bottom => new PointF((image.Width - textSize.Width) / 2, image.Height - textSize.Height - margin),
            Anchor.BottomRight => new PointF(image.Width - textSize.Width - margin, image.Height - textSize.Height - margin),
            _ => new PointF(image.Width - textSize.Width - margin, image.Height - textSize.Height - margin)
        };

        var color = new Rgba32(wm.Color.R, wm.Color.G, wm.Color.B, wm.Color.A);

        image.Mutate(ctx => ctx.DrawText(text, font, color, location));
    }

    /// <summary>
    /// Saves the image to the output stream in the specified format.
    /// </summary>
    /// <param name="image">The image to save.</param>
    /// <param name="output">The output stream.</param>
    /// <param name="options">Thumbnail options.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task SaveAsync(Image image, Stream output, ThumbnailOptions options, CancellationToken ct)
    {
        switch (options.Output.Kind.ToLowerInvariant())
        {
            case "png":
                var p = new PngEncoder { CompressionLevel = options.Output.PngCompressionLevel, SkipMetadata = true };
                await image.SaveAsync(output, p, ct);
                break;
            case "jpeg":
            default:
                var j = new JpegEncoder { Quality = options.Output.Quality, SkipMetadata = true };
                await image.SaveAsync(output, j, ct);
                break;
        }
    }
}
