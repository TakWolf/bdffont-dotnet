namespace BdfSpec.Tests.Conformance;

public class ParseDumpTests
{
    [Theory]
    [InlineData("demo", "demo.bdf")]
    [InlineData("misaki", "misaki_gothic.bdf")]
    [InlineData("misaki", "misaki_gothic_2nd.bdf")]
    [InlineData("misaki", "misaki_mincho.bdf")]
    [InlineData("unifont", "unifont-17.0.04.bdf")]
    public void TestParseDump(string fontDir, string fontFileName)
    {
        var data = File.ReadAllText(Path.Combine("assets", fontDir, fontFileName));
        var font = BdfFont.Parse(data);
        Assert.Equal(data.Replace("\r\n", "\n").Replace("\nBITMAP \n", "\nBITMAP\n"), font.DumpToString());
    }
}
