using BdfSpec.Errors;

namespace BdfSpec.Tests;

public class BdfFontTests
{
    [Fact]
    public void TestFont1()
    {
        var font = new BdfFont();

        font.Resolution = (1, 2);
        Assert.Equal((1, 2), font.Resolution);
        Assert.Equal(1, font.ResolutionX);
        Assert.Equal(2, font.ResolutionY);

        font.Dimensions = (3, 4);
        Assert.Equal((3, 4), font.Dimensions);
        Assert.Equal(3, font.Width);
        Assert.Equal(4, font.Height);

        font.Offset = (5, 6);
        Assert.Equal((5, 6), font.Offset);
        Assert.Equal(5, font.OffsetX);
        Assert.Equal(6, font.OffsetY);
        Assert.Equal((3, 4, 5, 6), font.BoundingBox);

        font.BoundingBox = (7, 8, 9, 10);
        Assert.Equal((7, 8, 9, 10), font.BoundingBox);
        Assert.Equal(7, font.Width);
        Assert.Equal(8, font.Height);
        Assert.Equal(9, font.OffsetX);
        Assert.Equal(10, font.OffsetY);
    }

    [Fact]
    public void TestFont2()
    {
        var font = new BdfFont();

        font.PointSize = 16;
        font.Resolution = (75, 75);

        font.Properties.Foundry = "TakWolf Studio";
        font.Properties.FamilyName = "Demo Pixel";
        font.Properties.WeightName = "Medium";
        font.Properties.Slant = "R";
        font.Properties.SetWidthName = "Normal";
        font.Properties.AddStyleName = "Sans Serif";
        font.Properties.PixelSize = font.PointSize;
        font.Properties.PointSize = font.PointSize * 10;
        font.Properties.ResolutionX = font.ResolutionX;
        font.Properties.ResolutionY = font.ResolutionY;
        font.Properties.Spacing = "P";
        font.Properties.AverageWidth = 80;
        font.Properties.CharsetRegistry = "ISO10646";
        font.Properties.CharsetEncoding = "1";

        font.GenerateNameAsXlfd();
        Assert.Equal("-TakWolf Studio-Demo Pixel-Medium-R-Normal-Sans Serif-16-160-75-75-P-80-ISO10646-1", font.Name);
    }

    [Fact]
    public void TestFont3()
    {
        var font = new BdfFont();

        var e = Assert.Throws<BdfXlfdException>(() => font.UpdateByNameAsXlfd());
        Assert.Equal("Not starts with '-'.", e.Message);

        font.Name = "--------------";
        font.UpdateByNameAsXlfd();
        Assert.Equal(0, font.ResolutionX);
        Assert.Equal(0, font.ResolutionY);
        Assert.Empty(font.Properties);

        font.Name = "-Adobe-Times-Medium-R-Normal--14-100-100-100-P-74-ISO8859-1";
        font.UpdateByNameAsXlfd();
        Assert.Equal(100, font.ResolutionX);
        Assert.Equal(100, font.ResolutionY);
        Assert.Equal("Adobe", font.Properties.Foundry);
        Assert.Equal("Times", font.Properties.FamilyName);
        Assert.Equal("Medium", font.Properties.WeightName);
        Assert.Equal("R", font.Properties.Slant);
        Assert.Equal("Normal", font.Properties.SetWidthName);
        Assert.Null(font.Properties.AddStyleName);
        Assert.Equal(14, font.Properties.PixelSize);
        Assert.Equal(100, font.Properties.PointSize);
        Assert.Equal(100, font.Properties.ResolutionX);
        Assert.Equal(100, font.Properties.ResolutionY);
        Assert.Equal("P", font.Properties.Spacing);
        Assert.Equal(74, font.Properties.AverageWidth);
        Assert.Equal("ISO8859", font.Properties.CharsetRegistry);
        Assert.Equal("1", font.Properties.CharsetEncoding);
    }

    [Fact]
    public void TestDemo()
    {
        var font = BdfFont.Load(Path.Combine("assets", "demo", "demo.bdf"));

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
    public void TestMultiLine2()
    {
        var font = new BdfFont();
        font.Properties["ABC"] = "Hello\nWorld";
        var e = Assert.Throws<BdfDumpException>(() => font.DumpToString());
        Assert.Equal("Property value cannot be multi-line string.", e.Message);
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
    public void TestParseBitmap1()
    {
        var font = BdfFont.Parse(
            """
            STARTFONT 2.1
            FONT
            SIZE 0 0 0
            FONTBOUNDINGBOX 0 0 0 0
            STARTPROPERTIES 0
            ENDPROPERTIES
            CHARS 1
            STARTCHAR _
            ENCODING 0
            SWIDTH 0 0
            DWIDTH 0 0
            BBX 10 1 0 0
            BITMAP
            FF
            ENDCHAR
            ENDFONT
            """);
        Assert.Equal([
            [1, 1, 1, 1, 1, 1, 1, 1, 0, 0]
        ], font.Glyphs[0].Bitmap);
    }

    [Fact]
    public void TestParseBitmap2()
    {
        var font = BdfFont.Parse(
            """
            STARTFONT 2.1
            FONT
            SIZE 0 0 0
            FONTBOUNDINGBOX 0 0 0 0
            STARTPROPERTIES 0
            ENDPROPERTIES
            CHARS 1
            STARTCHAR _
            ENCODING 0
            SWIDTH 0 0
            DWIDTH 0 0
            BBX 6 1 0 0
            BITMAP
            FF
            ENDCHAR
            ENDFONT
            """);
        Assert.Equal([
            [1, 1, 1, 1, 1, 1]
        ], font.Glyphs[0].Bitmap);
    }

    [Fact]
    public void TestDumpBitmap1()
    {
        var font = new BdfFont();
        font.Glyphs.Add(new BdfGlyph(
            name: "_",
            encoding: 0,
            boundingBox: (10, 1, 0, 0),
            bitmap: [
                [2, 2, 2, 2, 2, 2]
            ]));
        Assert.Equal(
            """
            STARTFONT 2.1
            FONT
            SIZE 0 0 0
            FONTBOUNDINGBOX 0 0 0 0
            STARTPROPERTIES 0
            ENDPROPERTIES
            CHARS 1
            STARTCHAR _
            ENCODING 0
            SWIDTH 0 0
            DWIDTH 0 0
            BBX 10 1 0 0
            BITMAP
            FC00
            ENDCHAR
            ENDFONT
            """.Replace("\r\n", "\n"), font.DumpToString().TrimEnd());
    }

    [Fact]
    public void TestDumpBitmap2()
    {
        var font = new BdfFont();
        font.Glyphs.Add(new BdfGlyph(
            name: "_",
            encoding: 0,
            boundingBox: (6, 1, 0, 0),
            bitmap: [
                [2, 2, 2, 2, 2, 2, 2, 2]
            ]));
        Assert.Equal(
            """
            STARTFONT 2.1
            FONT
            SIZE 0 0 0
            FONTBOUNDINGBOX 0 0 0 0
            STARTPROPERTIES 0
            ENDPROPERTIES
            CHARS 1
            STARTCHAR _
            ENCODING 0
            SWIDTH 0 0
            DWIDTH 0 0
            BBX 6 1 0 0
            BITMAP
            FC
            ENDCHAR
            ENDFONT
            """.Replace("\r\n", "\n"), font.DumpToString().TrimEnd());
    }

    [Fact]
    public void TestCopy()
    {
        var font1 = BdfFont.Load(Path.Combine("assets", "demo", "demo.bdf"));
        var font2 = font1.Copy();

        Assert.Equal(font1, font2);
        Assert.NotSame(font1, font2);
        Assert.Same(font1.Properties, font2.Properties);
        Assert.Same(font1.Glyphs, font2.Glyphs);
        Assert.Same(font1.Comments, font2.Comments);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var font1 = BdfFont.Load(Path.Combine("assets", "demo", "demo.bdf"));
        var font2 = font1.DeepCopy();

        Assert.Equal(font1, font2);
        Assert.NotSame(font1, font2);
        Assert.NotSame(font1.Properties, font2.Properties);
        Assert.NotSame(font1.Glyphs, font2.Glyphs);
        Assert.NotSame(font1.Comments, font2.Comments);

        foreach (var (glyph1, glyph2) in font1.Glyphs.Zip(font2.Glyphs))
        {
            Assert.NotSame(glyph1, glyph2);
        }
    }

    [Fact]
    public void TestEquals()
    {
        var filePath = Path.Combine("assets", "demo", "demo.bdf");
        var font1 = BdfFont.Load(filePath);
        var font2 = BdfFont.Load(filePath);
        Assert.Equal(font1, font2);
    }
}
