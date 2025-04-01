using BdfSpec.Internal;

namespace BdfSpec;

public class BdfFont
{
    public static BdfFont Parse(TextReader reader)
    {
        return BdfUtils.ParseReader(reader);
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

    public static async Task<BdfFont> ParseAsync(TextReader reader)
    {
        return await BdfUtils.ParseReaderAsync(reader);
    }

    public static async Task<BdfFont> ParseAsync(string text)
    {
        using var reader = new StringReader(text);
        return await ParseAsync(reader);
    }

    public static async Task<BdfFont> LoadAsync(string path)
    {
        using var reader = new StreamReader(path);
        return await ParseAsync(reader);
    }

    public string Name;
    public int PointSize;
    public int ResolutionX;
    public int ResolutionY;
    public int Width;
    public int Height;
    public int OffsetX;
    public int OffsetY;
    public BdfProperties Properties;
    public List<BdfGlyph> Glyphs;
    public List<string> Comments;

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
        BdfUtils.DumpWriter(writer, this);
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

    public async Task DumpAsync(TextWriter writer)
    {
        await BdfUtils.DumpWriterAsync(writer, this);
    }

    public async Task<string> DumpToStringAsync()
    {
        await using var writer = new StringWriter();
        await DumpAsync(writer);
        return writer.ToString();
    }

    public async Task SaveAsync(string path)
    {
        await using var writer = new StreamWriter(path);
        await DumpAsync(writer);
    }
}
