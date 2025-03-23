namespace BdfSpec.Tests;

public class GlyphTests
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
}
