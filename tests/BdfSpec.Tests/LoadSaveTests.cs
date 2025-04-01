namespace BdfSpec.Tests;

public class LoadSaveTests : IClassFixture<PathFixture>
{
    [Fact]
    public void TestUnifont()
    {
        var loadPath = Path.Combine(PathDefine.AssetsDir, "unifont", "unifont-16.0.02.bdf");
        var savePath = Path.Combine(PathDefine.CreateTmpDir(), "unifont-16.0.02.bdf");
        var font = BdfFont.Load(loadPath);
        File.WriteAllText(savePath, font.DumpToString().Replace("\nBITMAP\n", "\nBITMAP \n"));
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r", ""), File.ReadAllText(savePath));
    }

    [Fact]
    public async Task TestUnifontAsync()
    {
        var loadPath = Path.Combine(PathDefine.AssetsDir, "unifont", "unifont-16.0.02.bdf");
        var savePath = Path.Combine(PathDefine.CreateTmpDir(), "unifont-16.0.02.bdf");
        var font = await BdfFont.LoadAsync(loadPath);
        await File.WriteAllTextAsync(savePath, (await font.DumpToStringAsync()).Replace("\nBITMAP\n", "\nBITMAP \n"));
        Assert.Equal((await File.ReadAllTextAsync(loadPath)).Replace("\r", ""), await File.ReadAllTextAsync(savePath));
    }

    [Fact]
    public void TestMisakiGothic()
    {
        var loadPath = Path.Combine(PathDefine.AssetsDir, "misaki", "misaki_gothic.bdf");
        var savePath = Path.Combine(PathDefine.CreateTmpDir(), "misaki_gothic.bdf");
        var font = BdfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r", ""), File.ReadAllText(savePath));
    }

    [Fact]
    public async Task TestMisakiGothicAsync()
    {
        var loadPath = Path.Combine(PathDefine.AssetsDir, "misaki", "misaki_gothic.bdf");
        var savePath = Path.Combine(PathDefine.CreateTmpDir(), "misaki_gothic.bdf");
        var font = await BdfFont.LoadAsync(loadPath);
        await font.SaveAsync(savePath);
        Assert.Equal((await File.ReadAllTextAsync(loadPath)).Replace("\r", ""), await File.ReadAllTextAsync(savePath));
    }

    [Fact]
    public void TestMisakiGothic2Nd()
    {
        var loadPath = Path.Combine(PathDefine.AssetsDir, "misaki", "misaki_gothic_2nd.bdf");
        var savePath = Path.Combine(PathDefine.CreateTmpDir(), "misaki_gothic_2nd.bdf");
        var font = BdfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r", ""), File.ReadAllText(savePath));
    }

    [Fact]
    public async Task TestMisakiGothic2NdAsync()
    {
        var loadPath = Path.Combine(PathDefine.AssetsDir, "misaki", "misaki_gothic_2nd.bdf");
        var savePath = Path.Combine(PathDefine.CreateTmpDir(), "misaki_gothic_2nd.bdf");
        var font = await BdfFont.LoadAsync(loadPath);
        await font.SaveAsync(savePath);
        Assert.Equal((await File.ReadAllTextAsync(loadPath)).Replace("\r", ""), await File.ReadAllTextAsync(savePath));
    }

    [Fact]
    public void TestMisakiMincho()
    {
        var loadPath = Path.Combine(PathDefine.AssetsDir, "misaki", "misaki_mincho.bdf");
        var savePath = Path.Combine(PathDefine.CreateTmpDir(), "misaki_mincho.bdf");
        var font = BdfFont.Load(loadPath);
        font.Save(savePath);
        Assert.Equal(File.ReadAllText(loadPath).Replace("\r", ""), File.ReadAllText(savePath));
    }

    [Fact]
    public async Task TestMisakiMinchoAsync()
    {
        var loadPath = Path.Combine(PathDefine.AssetsDir, "misaki", "misaki_mincho.bdf");
        var savePath = Path.Combine(PathDefine.CreateTmpDir(), "misaki_mincho.bdf");
        var font = await BdfFont.LoadAsync(loadPath);
        await font.SaveAsync(savePath);
        Assert.Equal((await File.ReadAllTextAsync(loadPath)).Replace("\r", ""), await File.ReadAllTextAsync(savePath));
    }
}
