namespace BdfSpec;

public class BdfGlyph
{
    public string Name { get; set; }
    public int Encoding { get; set; }
    public int ScalableWidthX { get; set; }
    public int ScalableWidthY { get; set; }
    public int DeviceWidthX { get; set; }
    public int DeviceWidthY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public List<List<byte>> Bitmap { get; set; }
    public List<string> Comments { get; set; }

    public BdfGlyph(
        string name,
        int encoding,
        (int, int) scalableWidth = default,
        (int, int) deviceWidth = default,
        (int, int, int, int) boundingBox = default,
        List<List<byte>>? bitmap = null,
        List<string>? comments = null)
    {
        Name = name;
        Encoding = encoding;
        (ScalableWidthX, ScalableWidthY) = scalableWidth;
        (DeviceWidthX, DeviceWidthY) = deviceWidth;
        (Width, Height, OffsetX, OffsetY) = boundingBox;
        Bitmap = bitmap ?? [];
        Comments = comments ?? [];
    }

    public (int, int) ScalableWidth
    {
        get => (ScalableWidthX, ScalableWidthY);
        set => (ScalableWidthX, ScalableWidthY) = value;
    }

    public (int, int) DeviceWidth
    {
        get => (DeviceWidthX, DeviceWidthY);
        set => (DeviceWidthX, DeviceWidthY) = value;
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
}
