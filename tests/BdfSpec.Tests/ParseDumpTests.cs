using BdfSpec.Error;

namespace BdfSpec.Tests;

public class ParseDumpTests
{
    [Fact]
    public void TestDemo()
    {
        var data = File.ReadAllText(Path.Combine(PathDefine.AssetsDir, "demo.bdf"));
        var font = BdfFont.Parse(data);
        Assert.Equal(data.Replace("\r", ""), font.DumpToString());

        Assert.Equal("-Adobe-Helvetica-Bold-R-Normal--24-240-75-75-P-65-ISO8859-1", font.Name);
        Assert.Equal(24, font.PointSize);
        Assert.Equal(75, font.ResolutionX);
        Assert.Equal(75, font.ResolutionY);
        Assert.Equal((75, 75), font.Resolution);
        Assert.Equal(9, font.Width);
        Assert.Equal(24, font.Height);
        Assert.Equal((9, 24), font.Dimensions);
        Assert.Equal(-2, font.OffsetX);
        Assert.Equal(-6, font.OffsetY);
        Assert.Equal((-2, -6), font.Offset);
        Assert.Equal((9, 24, -2, -6), font.BoundingBox);
        Assert.Equal(["This is a sample font in 2.1 format."], font.Comments);

        Assert.Equal(19, font.Properties.Count);
        Assert.Equal("Adobe", font.Properties.Foundry);
        Assert.Equal("Helvetica", font.Properties.FamilyName);
        Assert.Equal("Bold", font.Properties.WeightName);
        Assert.Equal("R", font.Properties.Slant);
        Assert.Equal("Normal", font.Properties.SetWidthName);
        Assert.Equal("", font.Properties.AddStyleName);
        Assert.Equal(24, font.Properties.PixelSize);
        Assert.Equal(240, font.Properties.PointSize);
        Assert.Equal(75, font.Properties.ResolutionX);
        Assert.Equal(75, font.Properties.ResolutionY);
        Assert.Equal("P", font.Properties.Spacing);
        Assert.Equal(65, font.Properties.AverageWidth);
        Assert.Equal("ISO8859", font.Properties.CharsetRegistry);
        Assert.Equal("1", font.Properties.CharsetEncoding);
        Assert.Equal(4, font.Properties["MIN_SPACE"]);
        Assert.Equal(21, font.Properties.FontAscent);
        Assert.Equal(7, font.Properties.FontDescent);
        Assert.Equal("Copyright (c) 1987 Adobe Systems, Inc.", font.Properties.Copyright);
        Assert.Equal("Helvetica is a registered trademark of Linotype Inc.", font.Properties.Notice);
        Assert.Equal(["This is a comment in properties."], font.Properties.Comments);

        Assert.Equal(2, font.Glyphs.Count);
        var glyph = font.Glyphs.ToDictionary(glyph => glyph.Encoding, glyph => glyph)[39];
        Assert.Equal("quoteright", glyph.Name);
        Assert.Equal(39, glyph.Encoding);
        Assert.Equal(223, glyph.ScalableWidthX);
        Assert.Equal(0, glyph.ScalableWidthY);
        Assert.Equal((223, 0), glyph.ScalableWidth);
        Assert.Equal(5, glyph.DeviceWidthX);
        Assert.Equal(0, glyph.DeviceWidthY);
        Assert.Equal((5, 0), glyph.DeviceWidth);
        Assert.Equal(4, glyph.Width);
        Assert.Equal(6, glyph.Height);
        Assert.Equal((4, 6), glyph.Dimensions);
        Assert.Equal(2, glyph.OffsetX);
        Assert.Equal(12, glyph.OffsetY);
        Assert.Equal((2, 12), glyph.Offset);
        Assert.Equal((4, 6, 2, 12), glyph.BoundingBox);
        Assert.Equal([
            [0, 1, 1, 1],
            [0, 1, 1, 1],
            [0, 1, 1, 1],
            [0, 1, 1, 0],
            [1, 1, 1, 0],
            [1, 1, 0, 0]
        ], glyph.Bitmap);
        Assert.Equal(["This is a comment in char."], glyph.Comments);
    }

    [Fact]
    public async Task TestDemoAsync()
    {
        var data = await File.ReadAllTextAsync(Path.Combine(PathDefine.AssetsDir, "demo.bdf"));
        var font = await BdfFont.ParseAsync(data);
        Assert.Equal(data.Replace("\r", ""), await font.DumpToStringAsync());

        Assert.Equal("-Adobe-Helvetica-Bold-R-Normal--24-240-75-75-P-65-ISO8859-1", font.Name);
        Assert.Equal(24, font.PointSize);
        Assert.Equal(75, font.ResolutionX);
        Assert.Equal(75, font.ResolutionY);
        Assert.Equal((75, 75), font.Resolution);
        Assert.Equal(9, font.Width);
        Assert.Equal(24, font.Height);
        Assert.Equal((9, 24), font.Dimensions);
        Assert.Equal(-2, font.OffsetX);
        Assert.Equal(-6, font.OffsetY);
        Assert.Equal((-2, -6), font.Offset);
        Assert.Equal((9, 24, -2, -6), font.BoundingBox);
        Assert.Equal(["This is a sample font in 2.1 format."], font.Comments);

        Assert.Equal(19, font.Properties.Count);
        Assert.Equal("Adobe", font.Properties.Foundry);
        Assert.Equal("Helvetica", font.Properties.FamilyName);
        Assert.Equal("Bold", font.Properties.WeightName);
        Assert.Equal("R", font.Properties.Slant);
        Assert.Equal("Normal", font.Properties.SetWidthName);
        Assert.Equal("", font.Properties.AddStyleName);
        Assert.Equal(24, font.Properties.PixelSize);
        Assert.Equal(240, font.Properties.PointSize);
        Assert.Equal(75, font.Properties.ResolutionX);
        Assert.Equal(75, font.Properties.ResolutionY);
        Assert.Equal("P", font.Properties.Spacing);
        Assert.Equal(65, font.Properties.AverageWidth);
        Assert.Equal("ISO8859", font.Properties.CharsetRegistry);
        Assert.Equal("1", font.Properties.CharsetEncoding);
        Assert.Equal(4, font.Properties["MIN_SPACE"]);
        Assert.Equal(21, font.Properties.FontAscent);
        Assert.Equal(7, font.Properties.FontDescent);
        Assert.Equal("Copyright (c) 1987 Adobe Systems, Inc.", font.Properties.Copyright);
        Assert.Equal("Helvetica is a registered trademark of Linotype Inc.", font.Properties.Notice);
        Assert.Equal(["This is a comment in properties."], font.Properties.Comments);

        Assert.Equal(2, font.Glyphs.Count);
        var glyph = font.Glyphs.ToDictionary(glyph => glyph.Encoding, glyph => glyph)[39];
        Assert.Equal("quoteright", glyph.Name);
        Assert.Equal(39, glyph.Encoding);
        Assert.Equal(223, glyph.ScalableWidthX);
        Assert.Equal(0, glyph.ScalableWidthY);
        Assert.Equal((223, 0), glyph.ScalableWidth);
        Assert.Equal(5, glyph.DeviceWidthX);
        Assert.Equal(0, glyph.DeviceWidthY);
        Assert.Equal((5, 0), glyph.DeviceWidth);
        Assert.Equal(4, glyph.Width);
        Assert.Equal(6, glyph.Height);
        Assert.Equal((4, 6), glyph.Dimensions);
        Assert.Equal(2, glyph.OffsetX);
        Assert.Equal(12, glyph.OffsetY);
        Assert.Equal((2, 12), glyph.Offset);
        Assert.Equal((4, 6, 2, 12), glyph.BoundingBox);
        Assert.Equal([
            [0, 1, 1, 1],
            [0, 1, 1, 1],
            [0, 1, 1, 1],
            [0, 1, 1, 0],
            [1, 1, 1, 0],
            [1, 1, 0, 0]
        ], glyph.Bitmap);
        Assert.Equal(["This is a comment in char."], glyph.Comments);
    }

    [Fact]
    public void TestMultiLine1()
    {
        var font = new BdfFont();
        font.Comments.Add("Hello\nWorld");
        var e = Assert.Throws<BdfDumpException>(() => font.DumpToString());
        Assert.Equal("Tail cannot be multi-line string.", e.Message);
    }

    [Fact]
    public async Task TestMultiLine1Async()
    {
        var font = new BdfFont();
        font.Comments.Add("Hello\nWorld");
        var e = await Assert.ThrowsAsync<BdfDumpException>(async () => await font.DumpToStringAsync());
        Assert.Equal("Tail cannot be multi-line string.", e.Message);
    }

    [Fact]
    public void TestMultiLine2()
    {
        var font = new BdfFont();
        font.Properties["ABC"] = "Hello\nWorld";
        var e = Assert.Throws<BdfDumpException>(() => font.DumpToString());
        Assert.Equal("Properties value cannot be multi-line string.", e.Message);
    }

    [Fact]
    public async Task TestMultiLine2Async()
    {
        var font = new BdfFont();
        font.Properties["ABC"] = "Hello\nWorld";
        var e = await Assert.ThrowsAsync<BdfDumpException>(async () => await font.DumpToStringAsync());
        Assert.Equal("Properties value cannot be multi-line string.", e.Message);
    }

    [Fact]
    public void TestMultiLine3()
    {
        var font = new BdfFont();
        font.Properties.Comments.Add("Hello\nWorld");
        var e = Assert.Throws<BdfDumpException>(() => font.DumpToString());
        Assert.Equal("Tail cannot be multi-line string.", e.Message);
    }

    [Fact]
    public async Task TestMultiLine3Async()
    {
        var font = new BdfFont();
        font.Properties.Comments.Add("Hello\nWorld");
        var e = await Assert.ThrowsAsync<BdfDumpException>(async () => await font.DumpToStringAsync());
        Assert.Equal("Tail cannot be multi-line string.", e.Message);
    }

    [Fact]
    public void TestMultiLine4()
    {
        var font = new BdfFont();
        font.Glyphs.Add(new BdfGlyph(
            name: "A",
            encoding: 65,
            comments: ["Hello\nWorld"]));
        var e = Assert.Throws<BdfDumpException>(() => font.DumpToString());
        Assert.Equal("Tail cannot be multi-line string.", e.Message);
    }

    [Fact]
    public async Task TestMultiLine4Async()
    {
        var font = new BdfFont();
        font.Glyphs.Add(new BdfGlyph(
            name: "A",
            encoding: 65,
            comments: ["Hello\nWorld"]));
        var e = await Assert.ThrowsAsync<BdfDumpException>(async () => await font.DumpToStringAsync());
        Assert.Equal("Tail cannot be multi-line string.", e.Message);
    }
}
