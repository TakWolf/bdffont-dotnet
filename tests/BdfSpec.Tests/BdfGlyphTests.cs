namespace BdfSpec.Tests;

public class BdfGlyphTests
{
    [Fact]
    public void TestGlyph()
    {
        var glyph = new BdfGlyph(name: "A", encoding: 65);
        Assert.Equal("A", glyph.Name);
        Assert.Equal(65, glyph.Encoding);
        Assert.Equal((0, 0), glyph.ScalableWidth);
        Assert.Equal((0, 0), glyph.DeviceWidth);
        Assert.Equal((0, 0, 0, 0), glyph.BoundingBox);
        Assert.Equal(0, glyph.Attributes);
        Assert.Equal([], glyph.Bitmap);
        Assert.Equal([], glyph.Comments);

        glyph.ScalableWidth = (1, 2);
        Assert.Equal((1, 2), glyph.ScalableWidth);
        Assert.Equal(1, glyph.ScalableWidthX);
        Assert.Equal(2, glyph.ScalableWidthY);

        glyph.DeviceWidth = (3, 4);
        Assert.Equal((3, 4), glyph.DeviceWidth);
        Assert.Equal(3, glyph.DeviceWidthX);
        Assert.Equal(4, glyph.DeviceWidthY);

        glyph.Dimensions = (5, 6);
        Assert.Equal((5, 6), glyph.Dimensions);
        Assert.Equal(5, glyph.Width);
        Assert.Equal(6, glyph.Height);

        glyph.Offset = (7, 8);
        Assert.Equal((7, 8), glyph.Offset);
        Assert.Equal(7, glyph.OffsetX);
        Assert.Equal(8, glyph.OffsetY);

        glyph.BoundingBox = (9, 10, 11, 12);
        Assert.Equal((9, 10, 11, 12), glyph.BoundingBox);
        Assert.Equal(9, glyph.Width);
        Assert.Equal(10, glyph.Height);
        Assert.Equal(11, glyph.OffsetX);
        Assert.Equal(12, glyph.OffsetY);
    }

    [Fact]
    public void TestCopy()
    {
        var glyph1 = new BdfGlyph(
            name: "A",
            encoding: 65,
            scalableWidth: (1, 2),
            deviceWidth: (3, 4),
            boundingBox: (5, 6, 7, 8),
            attributes: 9,
            bitmap: [[1, 0, 0, 1]],
            comments: ["This is a comment."]);
        var glyph2 = glyph1.Copy();

        Assert.Equal(glyph1, glyph2);
        Assert.NotSame(glyph1, glyph2);
        Assert.Same(glyph1.Bitmap, glyph2.Bitmap);
        Assert.Same(glyph1.Comments, glyph2.Comments);
    }

    [Fact]
    public void TestDeepCopy()
    {
        var glyph1 = new BdfGlyph(
            name: "A",
            encoding: 65,
            scalableWidth: (1, 2),
            deviceWidth: (3, 4),
            boundingBox: (5, 6, 7, 8),
            attributes: 9,
            bitmap: [[1, 0, 0, 1]],
            comments: ["This is a comment."]);
        var glyph2 = glyph1.DeepCopy();

        Assert.Equal(glyph1, glyph2);
        Assert.NotSame(glyph1, glyph2);
        Assert.NotSame(glyph1.Bitmap, glyph2.Bitmap);
        Assert.NotSame(glyph1.Comments, glyph2.Comments);

        foreach (var (bitmapRow1, bitmapRow2) in glyph1.Bitmap.Zip(glyph2.Bitmap))
        {
            Assert.NotSame(bitmapRow1, bitmapRow2);
        }
    }

    [Fact]
    public void TestEquals()
    {
        var glyph1 = new BdfGlyph(
            name: "A",
            encoding: 65,
            scalableWidth: (1, 2),
            deviceWidth: (3, 4),
            boundingBox: (5, 6, 7, 8),
            attributes: 9,
            bitmap: [[1, 0, 0, 1]],
            comments: ["This is a comment."]);
        var glyph2 = new BdfGlyph(
            name: "A",
            encoding: 65,
            scalableWidth: (1, 2),
            deviceWidth: (3, 4),
            boundingBox: (5, 6, 7, 8),
            attributes: 9,
            bitmap: [[1, 0, 0, 1]],
            comments: ["This is a comment."]);
        Assert.Equal(glyph1, glyph2);
    }
}
