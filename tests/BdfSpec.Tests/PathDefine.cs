namespace BdfSpec.Tests;

public static class PathDefine
{
    private static readonly string ProjectRootDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", ".."));
    
    public static readonly string AssetsDir = Path.Combine(ProjectRootDir, "assets");
    public static readonly string TmpDir = Path.Combine(ProjectRootDir, "build", "tmp");
    
    public static string CreateTmpDir()
    {
        var tmpDir = Path.Combine(TmpDir, Guid.NewGuid().ToString());
        Directory.CreateDirectory(tmpDir);
        return tmpDir;
    }
}
