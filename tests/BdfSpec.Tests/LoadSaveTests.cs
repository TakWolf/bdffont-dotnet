namespace BdfSpec.Tests;

public class LoadSaveTests
{
    [Fact]
    public void TestUnifont()
    {
        var loadPath = Path.Combine("assets", "unifont", "unifont-16.0.02.bdf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "unifont-16.0.02.bdf");
        var font = BdfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r\n", "\n"), File.ReadAllText(savePath).Replace("\nBITMAP\n", "\nBITMAP \n"));
    }

    [Fact]
    public void TestMisakiGothic()
    {
        var loadPath = Path.Combine("assets", "misaki", "misaki_gothic.bdf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "misaki_gothic.bdf");
        var font = BdfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r\n", "\n"), File.ReadAllText(savePath));
    }

    [Fact]
    public void TestMisakiGothic2()
    {
        var loadPath = Path.Combine("assets", "misaki", "misaki_gothic_2nd.bdf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "misaki_gothic_2nd.bdf");
        var font = BdfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r\n", "\n"), File.ReadAllText(savePath));
    }

    [Fact]
    public void TestMisakiMincho()
    {
        var loadPath = Path.Combine("assets", "misaki", "misaki_mincho.bdf");
        var savePath = Path.Combine(PathUtils.CreateTempDir(), "misaki_mincho.bdf");
        var font = BdfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r\n", "\n"), File.ReadAllText(savePath));
    }
}
