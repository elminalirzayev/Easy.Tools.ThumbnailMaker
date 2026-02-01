[![Build & Test](https://github.com/elminalirzayev/Easy.Tools.ThumbnailMaker/actions/workflows/build.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.ThumbnailMaker/actions/workflows/build.yml)
[![Build & Release](https://github.com/elminalirzayev/Easy.Tools.ThumbnailMaker/actions/workflows/release.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.ThumbnailMaker/actions/workflows/release.yml)
[![Build & Nuget Publish](https://github.com/elminalirzayev/Easy.Tools.ThumbnailMaker/actions/workflows/nuget.yml/badge.svg)](https://github.com/elminalirzayev/Easy.Tools.ThumbnailMaker/actions/workflows/nuget.yml)
[![Release](https://img.shields.io/github/v/release/elminalirzayev/Easy.Tools.ThumbnailMaker)](https://github.com/elminalirzayev/Easy.Tools.ThumbnailMaker/releases)
[![License](https://img.shields.io/github/license/elminalirzayev/Easy.Tools.ThumbnailMaker)](https://github.com/elminalirzayev/Easy.Tools.ThumbnailMaker/blob/master/LICENSE.txt)
[![NuGet](https://img.shields.io/nuget/v/Easy.Tools.ThumbnailMaker.svg)](https://www.nuget.org/packages/Easy.Tools.ThumbnailMaker)

# Easy.Tools.ThumbnailMaker

Easy.Tools.ThumbnailMaker is a lightweight and flexible .NET library that enables quick and easy creation of thumbnails (small preview images), resizing images, and adding text watermarks in your applications.

---

## Features

- Asynchronous image loading and thumbnail generation  
- Automatic EXIF orientation correction (auto-orient)  
- Flexible resize modes: Fit, Cover, Pad, Crop, Contain  
- Background color and padding support  
- Text watermark with customizable position, font, color, size, and opacity  
- JPEG and PNG output formats  
- Metadata stripping options (EXIF, ICC, XMP profiles)  
- Minimal external dependencies, high performance  
- Cross‑platform (Windows, Linux, macOS)

---

## Installation

Install via NuGet:

```bash
dotnet add package Easy.Tools.ThumbnailMaker
```

---

## Usage

### Simple thumbnail creation

```csharp
using ThumbnailMaker;

// Input and output file paths
await using var input = File.OpenRead("test.jpeg");
await using var output = File.Create("test1.jpeg");

// Define thumbnail colors
var white = ThumbnailColor.White;
var red = new ThumbnailColor(255, 0, 0);
var semiTransparentBlue = new ThumbnailColor(0, 0, 255, 128);
var fromHex1 = ThumbnailColor.FromHex("#FF0000");        // RRGGBB
var fromHex2 = ThumbnailColor.FromHex("80FF0000");       // AARRGGBB

// Create a thumbnail with specific options
var opts = new ThumbnailOptions(320, 320)
{
    Mode = ThumbnailResizeMode.Cover,
    Output = OutputFormat.Jpeg(85),
    Watermark = Watermark.TextMark("© ACME", size: 18, opacity: 0.9f, color: red)

};

// Create the thumbnail
await Thumbnail.MakeAsync(input, output, opts);
```

---

## Details

### `Thumbnail.MakeAsync(Stream input, ThumbnailOptions options, CancellationToken ct = default)`

- `input`: Stream containing the source image  
- `options`: Thumbnail creation options (`ThumbnailOptions`)  
- `ct`: Cancellation token (optional)  
- Returns: Byte array of the created thumbnail image

### `Thumbnail.MakeAsync(Stream input, Stream output, ThumbnailOptions options, CancellationToken ct = default)`

- `input`: Stream containing the source image  
- `output`: Stream containing the output image  
- `options`: Thumbnail creation options (`ThumbnailOptions`)  
- `ct`: Cancellation token (optional)  
- Returns: Task representing the asynchronous operation

---

### `ThumbnailOptions`

- `int Width` - Thumbnail width (required)  
- `int Height` - Thumbnail height (required)  
- `ThumbnailResizeMode Mode` - Resize mode (Fit, Cover, Pad, Crop, Contain)  
- `Anchor Anchor` - Represents the anchor point for positioning elements in a thumbnail.
- `bool AutoOrient` - Apply EXIF orientation correction automatically  
- `bool StripMetadata` - Remove metadata from output  
- `ColorBackground? Background` - Background color (used in Pad mode)  
- `OutputFormat Output` - Output file format and quality settings  
- `Watermark Watermark` - Text watermark settings
- `bool PreventUpscale` -  Prevents upscaling of the image if the source is smaller than the target size

---

### `Watermark`

- `string Text` - Watermark text  
- `float FontSize` - Font size  
- `float Opacity` - Opacity (0 to 1)  
- `Anchor Position` - Position (e.g. BottomRight)  
- `int Margin` - Margin in pixels  
- `string FontFamily` - Font family name  
- `ThumbnailColor color` - Color (hex, Rgb, etc.)

---

## Supported ThumbnailResizeModes

| Mode     | Description                                     |
|----------|-------------------------------------------------|
| Fit      | Maintain aspect ratio, fit inside target size   |
| Cover    | Fill entire area, cropping if necessary         |
| Pad      | Maintain aspect ratio, add padding as needed    |
| Crop     | Crop to exact size, focusing on anchor position |
| Contain  | Maintain aspect ratio, scale down if needed     |

---

## Requirements

- .NET 6 or later  
- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) (internal dependency)  
- [SixLabors.Fonts](https://github.com/SixLabors/Fonts) (internal dependency)  
- [SixLabors.ImageSharp.Drawing](https://github.com/SixLabors/ImageSharp.Drawing) (internal dependency)  

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

### Third-Party Dependencies

This package uses the following Six Labors libraries:
- SixLabors.ImageSharp
- SixLabors.Fonts
- SixLabors.ImageSharp.Drawing

Six Labors libraries are licensed under the **Six Labors Split License**:
- **Apache 2.0** for open-source or small revenue projects (< $1M/year)
- **Commercial License** required for other use cases

More information: [Six Labors Pricing & Licensing](https://sixlabors.com/pricing/)

---

## Contributing

Contributions and suggestions are welcome. Please open an issue or submit a pull request.

---

## Contact

For questions, contact us via elmin.alirzayev@gmail.com or GitHub.

---

## License

This project is licensed under the MIT License.

---

© 2025 Elmin Alirzayev / Easy Code Tools

