using System;
using System.Globalization;
using SixLabors.ImageSharp.PixelFormats;

namespace ThumbnailMakers;

/// <summary>
/// Represents a color with RGBA channels for use in thumbnails and watermarks.
/// </summary>
public class ThumbnailColor : IEquatable<ThumbnailColor>
{
    /// <summary>
    /// Red channel value (0-255).
    /// </summary>
    public byte R { get; }
    /// <summary>
    /// Green channel value (0-255).
    /// </summary>
    public byte G { get; }
    /// <summary>
    /// Blue channel value (0-255).
    /// </summary>
    public byte B { get; }
    /// <summary>
    /// Alpha channel value (0-255).
    /// </summary>
    public byte A { get; }

    /// <summary>
    /// Gets a white color (RGBA: 255,255,255,255).
    /// </summary>
    public static ThumbnailColor White => new(255, 255, 255, 255);
    /// <summary>
    /// Gets a black color (RGBA: 0,0,0,255).
    /// </summary>
    public static ThumbnailColor Black => new(0, 0, 0, 255);

    /// <summary>
    /// Initializes a new instance of <see cref="ThumbnailColor"/>.
    /// </summary>
    /// <param name="r">Red channel (0-255).</param>
    /// <param name="g">Green channel (0-255).</param>
    /// <param name="b">Blue channel (0-255).</param>
    /// <param name="a">Alpha channel (0-255, default 255).</param>
    public ThumbnailColor(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// Creates a <see cref="ThumbnailColor"/> from a hex string (RRGGBB or AARRGGBB).
    /// </summary>
    /// <param name="hex">Hex color string.</param>
    /// <returns>A new <see cref="ThumbnailColor"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if hex is null or empty.</exception>
    /// <exception cref="FormatException">Thrown if hex format is invalid.</exception>
    public static ThumbnailColor FromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            throw new ArgumentException("Hex code can not be null.", nameof(hex));

        hex = hex.TrimStart('#');

        if (hex.Length == 6)
        {
            // RRGGBB
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new ThumbnailColor(r, g, b);
        }
        else if (hex.Length == 8)
        {
            // AARRGGBB
            byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
            return new ThumbnailColor(r, g, b, a);
        }
        else
        {
            throw new FormatException("Invalid hec code format. Expecten hex formats: RRGGBB or AARRGGBB.");
        }
    }

    /// <summary>
    /// Converts this color to an ImageSharp Rgba32 value.
    /// </summary>
    /// <returns>The <see cref="Rgba32"/> representation.</returns>
    public Rgba32 ToRgba32() => new Rgba32(R, G, B, A);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ThumbnailColor);

    /// <summary>
    /// Checks equality with another <see cref="ThumbnailColor"/>.
    /// </summary>
    /// <param name="other">Other color to compare.</param>
    /// <returns>True if equal, otherwise false.</returns>
    public bool Equals(ThumbnailColor? other)
    {
        if (other is null) return false;
        return R == other.R && G == other.G && B == other.B && A == other.A;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(R, G, B, A);

    /// <summary>
    /// Returns a string representation of the color.
    /// </summary>
    /// <returns>String in RGBA format.</returns>
    public override string ToString() => $"RGBA({R}, {G}, {B}, {A})";

    /// <summary>
    /// Checks equality between two <see cref="ThumbnailColor"/> instances.
    /// </summary>
    /// <param name="left">Left color.</param>
    /// <param name="right">Right color.</param>
    /// <returns>True if equal, otherwise false.</returns>
    public static bool operator ==(ThumbnailColor? left, ThumbnailColor? right) => Equals(left, right);
    /// <summary>
    /// Checks inequality between two <see cref="ThumbnailColor"/> instances.
    /// </summary>
    /// <param name="left">Left color.</param>
    /// <param name="right">Right color.</param>
    /// <returns>True if not equal, otherwise false.</returns>
    public static bool operator !=(ThumbnailColor? left, ThumbnailColor? right) => !Equals(left, right);
}
