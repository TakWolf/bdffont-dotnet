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

font.Properties.FontVersion = "1.0.0";
font.Properties.Copyright = "Copyright (c) TakWolf";

font.Save(Path.Combine(outputsDir, "my-font.bdf"));
