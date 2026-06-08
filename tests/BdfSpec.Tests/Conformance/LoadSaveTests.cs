using BdfSpec.Tests.TestUtils;

namespace BdfSpec.Tests.Conformance;

public class LoadSaveTests
{
    [Theory]
    [InlineData("demo", "demo.bdf")]
    [InlineData("misaki", "misaki_gothic.bdf")]
    [InlineData("misaki", "misaki_gothic_2nd.bdf")]
    [InlineData("misaki", "misaki_mincho.bdf")]
    [InlineData("unifont", "unifont-17.0.04.bdf")]
    public void TestLoadSave(string fontDir, string fontFileName)
    {
        var loadPath = Path.Combine("assets", fontDir, fontFileName);
        var savePath = Path.Combine(PathUtil.CreateTempDir(), fontFileName);
        var font = BdfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r\n", "\n").Replace("\nBITMAP \n", "\nBITMAP\n"), File.ReadAllText(savePath));
    }
}
