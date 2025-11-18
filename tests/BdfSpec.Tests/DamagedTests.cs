using BdfSpec.Errors;

namespace BdfSpec.Tests;

public class DamagedTests
{
    [Fact]
    public void TestNotBdf()
    {
        var e = Assert.Throws<BdfIllegalWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "not_a_bdf.bdf")));
        Assert.Equal("This", e.Word);
    }

    [Fact]
    public void TestNotSupportVersion()
    {
        var e = Assert.Throws<BdfParseException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "not_support_version.bdf")));
        Assert.Equal("Spec version not support: 2.2", e.Message);
    }

    [Fact]
    public void TestNoLineFont()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_font.bdf")));
        Assert.Equal("FONT", e.Word);
    }

    [Fact]
    public void TestNoLineSize()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_size.bdf")));
        Assert.Equal("SIZE", e.Word);
    }

    [Fact]
    public void TestNoLineFontBoundingBox()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_fontboundingbox.bdf")));
        Assert.Equal("FONTBOUNDINGBOX", e.Word);
    }

    [Fact]
    public void TestNoLineEndProperties()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_end_properties.bdf")));
        Assert.Equal("ENDPROPERTIES", e.Word);
    }

    [Fact]
    public void TestNoLineChars()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_chars.bdf")));
        Assert.Equal("CHARS", e.Word);
    }

    [Fact]
    public void TestNoLineEncoding()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_encoding.bdf")));
        Assert.Equal("ENCODING", e.Word);
    }

    [Fact]
    public void TestNoLineSWidth()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_swidth.bdf")));
        Assert.Equal("SWIDTH", e.Word);
    }

    [Fact]
    public void TestNoLineDWidth()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_dwidth.bdf")));
        Assert.Equal("DWIDTH", e.Word);
    }

    [Fact]
    public void TestNoLineBbx()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_bbx.bdf")));
        Assert.Equal("BBX", e.Word);
    }

    [Fact]
    public void TestNoLineEndChar()
    {
        var e = Assert.Throws<FormatException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_end_char.bdf")));
        Assert.Equal("Could not find any recognizable digits.", e.Message);
    }

    [Fact]
    public void TestNoLineEndFont()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_end_font.bdf")));
        Assert.Equal("ENDFONT", e.Word);
    }

    [Fact]
    public void TestIllegalWordInFont()
    {
        var e = Assert.Throws<BdfIllegalWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "illegal_word_in_font.bdf")));
        Assert.Equal("ABC", e.Word);
    }

    [Fact]
    public void TestIllegalWordInChar()
    {
        var e = Assert.Throws<BdfIllegalWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "illegal_word_in_char.bdf")));
        Assert.Equal("DEF", e.Word);
    }

    [Fact]
    public void TestIncorrectPropertiesCount()
    {
        var e = Assert.Throws<BdfCountException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "incorrect_properties_count.bdf")));
        Assert.Equal("STARTPROPERTIES", e.Word);
    }

    [Fact]
    public void TestIncorrectCharsCount()
    {
        var e = Assert.Throws<BdfCountException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "incorrect_chars_count.bdf")));
        Assert.Equal("CHARS", e.Word);
    }
}
