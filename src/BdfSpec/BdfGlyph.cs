namespace BdfSpec;

public class BdfGlyph
{
    public string Name;
    public int Encoding;
    public int ScalableWidthX;
    public int ScalableWidthY;
    public int DeviceWidthX;
    public int DeviceWidthY;
    public int Width;
    public int Height;
    public int OffsetX;
    public int OffsetY;
    public List<List<byte>> Bitmap;
    public List<string> Comments;

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
