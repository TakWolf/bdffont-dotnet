namespace BdfSpec.Tests;

public class PathFixture : IDisposable
{
    public void Dispose()
    {
        if (Directory.Exists(PathDefine.TmpDir))
        {
            Directory.Delete(PathDefine.TmpDir, true);
        }
    }
}
