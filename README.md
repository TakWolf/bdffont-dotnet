# BdfFont.NET

[![.NET](https://img.shields.io/badge/dotnet-8.0-mediumpurple)](https://dotnet.microsoft.com)
[![NuGet](https://img.shields.io/nuget/v/BdfFont)](https://www.nuget.org/packages/BdfFont)

BdfFont is a library for manipulating [Glyph Bitmap Distribution Format (BDF) Fonts](https://en.wikipedia.org/wiki/Glyph_Bitmap_Distribution_Format).

## Installation

```shell
dotnet add package BdfFont
```

## Usage

### Create

```csharp
using BdfSpec;

var outputsDir = Path.Combine("build");
if (Directory.Exists(outputsDir))
{
    Directory.Delete(outputsDir, true);
}
Directory.CreateDirectory(outputsDir);

var font = new BdfFont(
    pointSize: 16,
    resolution: (75, 75),
    boundingBox: (16, 16, 0, -2));

font.Glyphs.Add(new BdfGlyph(
    name: "A",
    encoding: 65,
    scalableWidth: (500, 0),
    deviceWidth: (8, 0),
    boundingBox: (8, 16, 0, -2),
    bitmap: [
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 1, 1, 0, 0, 0],
        [0, 0, 1, 0, 0, 1, 0, 0],
        [0, 0, 1, 0, 0, 1, 0, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 1, 1, 1, 1, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 1, 0, 0, 0, 0, 1, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0]
    ]));

font.Properties.Foundry = "Pixel Font Studio";
font.Properties.FamilyName = "My Font";
font.Properties.WeightName = "Medium";
font.Properties.Slant = "R";
font.Properties.SetWidthName = "Normal";
font.Properties.AddStyleName = "Sans Serif";
font.Properties.PixelSize = font.PointSize;
font.Properties.PointSize = font.PointSize * 10;
font.Properties.ResolutionX = font.ResolutionX;
font.Properties.ResolutionY = font.ResolutionY;
font.Properties.Spacing = "P";
font.Properties.AverageWidth = Convert.ToInt32(font.Glyphs.Average(glyph => glyph.DeviceWidthX * 10));
font.Properties.CharsetRegistry = "ISO10646";
font.Properties.CharsetEncoding = "1";
font.GenerateNameAsXlfd();

font.Properties.DefaultChar = -1;
font.Properties.FontAscent = 14;
font.Properties.FontDescent = 2;
font.Properties.XHeight = 7;
font.Properties.CapHeight = 10;
font.Properties.UnderlinePosition = -2;
font.Properties.UnderlineThickness = 1;

font.Properties.FontVersion = "1.0.0";
font.Properties.Copyright = "Copyright (c) TakWolf";

font.Save(Path.Combine(outputsDir, "my-font.bdf"));
```

### Load

```csharp
using BdfSpec;

var outputsDir = Path.Combine("build");
if (Directory.Exists(outputsDir))
{
    Directory.Delete(outputsDir, true);
}
Directory.CreateDirectory(outputsDir);

var font = BdfFont.Load(Path.Combine("assets", "unifont", "unifont-17.0.03.bdf"));
Console.WriteLine($"name: {font.Name}");
Console.WriteLine($"size: {font.PointSize}");
Console.WriteLine($"ascent: {font.Properties.FontAscent}");
Console.WriteLine($"descent: {font.Properties.FontDescent}");
Console.WriteLine();
foreach (var glyph in font.Glyphs)
{
    Console.WriteLine($"char: {char.ConvertFromUtf32(glyph.Encoding)} ({glyph.Encoding:X4})");
    Console.WriteLine($"glyphName: {glyph.Name}");
    Console.WriteLine($"advanceWidth: {glyph.DeviceWidthX}");
    Console.WriteLine($"dimensions: {glyph.Dimensions}");
    Console.WriteLine($"offset: {glyph.Offset}");
    foreach (var bitmapRow in glyph.Bitmap)
    {
        var text = string.Join("", bitmapRow).Replace("0", "  ").Replace("1", "██");
        Console.WriteLine($"{text}*");
    }
    Console.WriteLine();
}
font.Save(Path.Combine(outputsDir, "unifont-17.0.03.bdf"));
```

## References

- [X11 - Bitmap Distribution Format - Version 2.1](https://www.x.org/docs/BDF/bdf.pdf)
- [Adobe - Glyph Bitmap Distribution Format (BDF) Specification - Version 2.2](https://adobe-type-tools.github.io/font-tech-notes/pdfs/5005.BDF_Spec.pdf)
- [X Logical Font Description Conventions - X Consortium Standard](https://www.x.org/releases/current/doc/xorg-docs/xlfd/xlfd.html)

## License

[MIT License](LICENSE)
