using BdfSpec.Utils;

namespace BdfSpec;

public class BdfFont
{
    public static BdfFont Parse(TextReader reader)
    {
        return BdfUtil.ParseReader(reader);
    }

    public static BdfFont Parse(string text)
    {
        using var reader = new StringReader(text);
        return Parse(reader);
    }

    public static BdfFont Load(string path)
    {
        using var reader = new StreamReader(path);
        return Parse(reader);
    }

    public string Name { get; set; }
    public int PointSize { get; set; }
    public int ResolutionX { get; set; }
    public int ResolutionY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public BdfProperties Properties { get; set; }
    public List<BdfGlyph> Glyphs { get; set; }
    public List<string> Comments { get; set; }

    public BdfFont(
        string name = "",
        int pointSize = 0,
        (int, int) resolution = default,
        (int, int, int, int) boundingBox = default,
        BdfProperties? properties = null,
        List<BdfGlyph>? glyphs = null,
        List<string>? comments = null)
    {
        Name = name;
        PointSize = pointSize;
        (ResolutionX, ResolutionY) = resolution;
        (Width, Height, OffsetX, OffsetY) = boundingBox;
        Properties = properties ?? new BdfProperties();
        Glyphs = glyphs ?? [];
        Comments = comments ?? [];
    }

    public (int, int) Resolution
    {
        get => (ResolutionX, ResolutionY);
        set => (ResolutionX, ResolutionY) = value;
    }

    public (int, int) Dimensions
    {
        get => (Width, Height);
        set => (Width, Height) = value;
    }

    public (int, int) Offset
    {
        get => (OffsetX, OffsetY);
        set => (OffsetX, OffsetY) = value;
    }

    public (int, int, int, int) BoundingBox
    {
        get => (Width, Height, OffsetX, OffsetY);
        set => (Width, Height, OffsetX, OffsetY) = value;
    }

    public void GenerateNameAsXlfd()
    {
        Name = Properties.ToXlfd();
    }

    public void UpdateByNameAsXlfd()
    {
        Properties.UpdateByXlfd(Name);
        ResolutionX = Properties.ResolutionX ?? 0;
        ResolutionY = Properties.ResolutionY ?? 0;
    }

    public void Dump(TextWriter writer)
    {
        BdfUtil.DumpWriter(writer, this);
    }

    public string DumpToString()
    {
        using var writer = new StringWriter();
        Dump(writer);
        return writer.ToString();
    }

    public void Save(string path)
    {
        using var writer = new StreamWriter(path);
        Dump(writer);
    }
}
