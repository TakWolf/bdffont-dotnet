using BdfSpec;

var outputsDir = Path.Combine("build");
if (Directory.Exists(outputsDir))
{
    Directory.Delete(outputsDir, true);
}
Directory.CreateDirectory(outputsDir);

var font = BdfFont.Load(Path.Combine("assets", "unifont", "unifont-16.0.02.bdf"));
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
font.Save(Path.Combine(outputsDir, "unifont-16.0.02.bdf"));
