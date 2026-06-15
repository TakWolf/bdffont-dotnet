using BdfSpec.Errors;

namespace BdfSpec.Tests.Conformance;

public class DamagedTests
{
    [Fact]
    public void TestNotBdf()
    {
        var e = Assert.Throws<BdfIllegalWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "not_bdf.bdf")));
        Assert.Equal("This", e.Word);
        Assert.Equal("Illegal word: This", e.Message);
    }

    [Fact]
    public void TestUnsupportedVersion()
    {
        var e = Assert.Throws<BdfParseException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "unsupported_version.bdf")));
        Assert.Equal("Unsupported BDF version: '2.2'", e.Message);
    }

    [Fact]
    public void TestNoLineFont()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_font.bdf")));
        Assert.Equal("FONT", e.Word);
        Assert.Equal("Missing word: FONT", e.Message);
    }

    [Fact]
    public void TestNoLineSize()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_size.bdf")));
        Assert.Equal("SIZE", e.Word);
        Assert.Equal("Missing word: SIZE", e.Message);
    }

    [Fact]
    public void TestNoLineFontBoundingBox()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_fontboundingbox.bdf")));
        Assert.Equal("FONTBOUNDINGBOX", e.Word);
        Assert.Equal("Missing word: FONTBOUNDINGBOX", e.Message);
    }

    [Fact]
    public void TestNoLineEndProperties()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_end_properties.bdf")));
        Assert.Equal("ENDPROPERTIES", e.Word);
        Assert.Equal("Missing word: ENDPROPERTIES", e.Message);
    }

    [Fact]
    public void TestNoLineChars()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_chars.bdf")));
        Assert.Equal("CHARS", e.Word);
        Assert.Equal("Missing word: CHARS", e.Message);
    }

    [Fact]
    public void TestNoLineEncoding()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_encoding.bdf")));
        Assert.Equal("ENCODING", e.Word);
        Assert.Equal("Missing word: ENCODING", e.Message);
    }

    [Fact]
    public void TestNoLineSWidth()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_swidth.bdf")));
        Assert.Equal("SWIDTH", e.Word);
        Assert.Equal("Missing word: SWIDTH", e.Message);
    }

    [Fact]
    public void TestNoLineDWidth()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_dwidth.bdf")));
        Assert.Equal("DWIDTH", e.Word);
        Assert.Equal("Missing word: DWIDTH", e.Message);
    }

    [Fact]
    public void TestNoLineBbx()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_bbx.bdf")));
        Assert.Equal("BBX", e.Word);
        Assert.Equal("Missing word: BBX", e.Message);
    }

    [Fact]
    public void TestNoLineEndChar()
    {
        var e = Assert.Throws<FormatException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_end_char.bdf")));
        Assert.Equal("The input string 'S' was not in a correct format.", e.Message);
    }

    [Fact]
    public void TestNoLineEndFont()
    {
        var e = Assert.Throws<BdfMissingWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "no_line_end_font.bdf")));
        Assert.Equal("ENDFONT", e.Word);
        Assert.Equal("Missing word: ENDFONT", e.Message);
    }

    [Fact]
    public void TestIllegalWordInFont()
    {
        var e = Assert.Throws<BdfIllegalWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "illegal_word_in_font.bdf")));
        Assert.Equal("ABC", e.Word);
        Assert.Equal("Illegal word: ABC", e.Message);
    }

    [Fact]
    public void TestIllegalWordInChar()
    {
        var e = Assert.Throws<BdfIllegalWordException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "illegal_word_in_char.bdf")));
        Assert.Equal("DEF", e.Word);
        Assert.Equal("Illegal word: DEF", e.Message);
    }

    [Fact]
    public void TestIncorrectPropertiesCount()
    {
        var e = Assert.Throws<BdfCountException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "incorrect_properties_count.bdf")));
        Assert.Equal("STARTPROPERTIES", e.Word);
        Assert.Equal("The count of STARTPROPERTIES is incorrect: 1000 -> 19", e.Message);
    }

    [Fact]
    public void TestIncorrectCharsCount()
    {
        var e = Assert.Throws<BdfCountException>(() => BdfFont.Load(Path.Combine("assets", "damaged", "incorrect_chars_count.bdf")));
        Assert.Equal("CHARS", e.Word);
        Assert.Equal("The count of CHARS is incorrect: 1000 -> 2", e.Message);
    }
}
