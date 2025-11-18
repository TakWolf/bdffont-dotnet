using BdfSpec.Errors;

namespace BdfSpec.Tests;

public class FontTests
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
}
