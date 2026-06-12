using System.Globalization;
using System.Text.RegularExpressions;
using BdfSpec.Errors;

namespace BdfSpec.Utils;

internal static partial class BdfUtil
{
    private const string SpecVersion = "2.1";

    private const string WordStartFont = "STARTFONT";
    private const string WordEndFont = "ENDFONT";
    private const string WordComment = "COMMENT";
    private const string WordFont = "FONT";
    private const string WordSize = "SIZE";
    private const string WordFontBoundingBox = "FONTBOUNDINGBOX";
    private const string WordStartProperties = "STARTPROPERTIES";
    private const string WordEndProperties = "ENDPROPERTIES";
    private const string WordChars = "CHARS";
    private const string WordStartChar = "STARTCHAR";
    private const string WordEndChar = "ENDCHAR";
    private const string WordEncoding = "ENCODING";
    private const string WordSWidth = "SWIDTH";
    private const string WordDWidth = "DWIDTH";
    private const string WordBbx = "BBX";
    private const string WordBitmap = "BITMAP";

    private const string CommentLinePrefix = $"{WordComment} ";

    private static readonly char[] NewLineChars = ['\n', '\r'];

    private static readonly char[] HexUpperChars = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'];

    [GeneratedRegex(@"\s+")]
    private static partial Regex RegexBlanks();

    private static IEnumerator<(string, string)> CreateLinesEnumerator(TextReader reader)
    {
        while (reader.ReadLine() is { } line)
        {
            line = line.Trim();
            if (line is "")
            {
                continue;
            }

            string word;
            string tail;
            if (line.StartsWith(CommentLinePrefix))
            {
                word = WordComment;
                tail = line[CommentLinePrefix.Length..];
            }
            else
            {
                var parts = RegexBlanks().Split(line, 2);
                word = parts[0];
                tail = parts.Length >= 2 ? parts[1] : "";
            }
            yield return (word, tail);
        }
    }

    private static void ConvertTailToInts(string tail, Span<int> values)
    {
        var parts = RegexBlanks().Split(tail);
        for (var i = 0; i < parts.Length; i++)
        {
            values[i] = int.Parse(parts[i]);
        }
    }

    private static BdfPropertyValue ConvertTailToPropertyValue(string tail)
    {
        if (tail.StartsWith('"') && tail.EndsWith('"'))
        {
            return tail[1..^1].Replace("\"\"", "\"");
        }

        if (int.TryParse(tail, out var value))
        {
            return value;
        }

        return tail;
    }

    private static BdfProperties ParsePropertiesSegment(IEnumerator<(string, string)> lines, int count)
    {
        var properties = new BdfProperties(count);
        while (lines.MoveNext())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordEndProperties:
                    if (properties.Count != count)
                    {
                        throw BdfCountException.Create(WordStartProperties, count, properties.Count);
                    }
                    return properties;
                case WordComment:
                    properties.Comments.Add(tail);
                    break;
                default:
                    var value = ConvertTailToPropertyValue(tail);
                    try
                    {
                        properties[word] = value;
                    }
                    catch (BdfValueException)
                    {
                        properties[word] = value.ToString();
                    }
                    break;
            }
        }
        throw BdfMissingWordException.Create(WordEndProperties);
    }

    private static List<List<byte>> ParseBitmapSegment(IEnumerator<(string, string)> lines, int glyphWidth, int glyphHeight)
    {
        var bitmap = new List<List<byte>>(glyphHeight);
        while (lines.MoveNext())
        {
            var (word, _) = lines.Current;
            switch (word)
            {
                case WordEndChar:
                    return bitmap;
                default:
                    var bitmapRow = new List<byte>(glyphWidth);
                    for (var i = 0; i < glyphWidth; i += 4)
                    {
                        var start = i / 4;
                        var b = start < word.Length ? byte.Parse(word.AsSpan(start, 1), NumberStyles.HexNumber) : 0;
                        for (var shift = 0; shift < 4 && i + shift < glyphWidth; shift++)
                        {
                            bitmapRow.Add((byte)((b >> (3 - shift)) & 1));
                        }
                    }
                    bitmap.Add(bitmapRow);
                    break;
            }
        }
        throw BdfMissingWordException.Create(WordEndChar);
    }

    private static BdfGlyph ParseGlyphSegment(IEnumerator<(string, string)> lines, string name)
    {
        int? encoding = null;
        (int, int)? scalableWidth = null;
        (int, int)? deviceWidth = null;
        (int, int, int, int)? boundingBox = null;
        List<string> comments = [];

        Span<int> intValues = stackalloc int[4];
        while (lines.MoveNext())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordEncoding:
                    encoding = int.Parse(tail);
                    break;
                case WordSWidth:
                    ConvertTailToInts(tail, intValues);
                    scalableWidth = (intValues[0], intValues[1]);
                    break;
                case WordDWidth:
                    ConvertTailToInts(tail, intValues);
                    deviceWidth = (intValues[0], intValues[1]);
                    break;
                case WordBbx:
                    ConvertTailToInts(tail, intValues);
                    boundingBox = (intValues[0], intValues[1], intValues[2], intValues[3]);
                    break;
                case WordComment:
                    comments.Add(tail);
                    break;
                case WordBitmap:
                case WordEndChar:
                    if (encoding is null)
                    {
                        throw BdfMissingWordException.Create(WordEncoding);
                    }
                    if (scalableWidth is null)
                    {
                        throw BdfMissingWordException.Create(WordSWidth);
                    }
                    if (deviceWidth is null)
                    {
                        throw BdfMissingWordException.Create(WordDWidth);
                    }
                    if (boundingBox is null)
                    {
                        throw BdfMissingWordException.Create(WordBbx);
                    }
                    List<List<byte>>? bitmap = null;
                    if (word is WordBitmap)
                    {
                        bitmap = ParseBitmapSegment(lines, boundingBox.Value.Item1, boundingBox.Value.Item2);
                    }
                    return new BdfGlyph(
                        name,
                        encoding.Value,
                        scalableWidth.Value,
                        deviceWidth.Value,
                        boundingBox.Value,
                        bitmap,
                        comments);
                default:
                    throw BdfIllegalWordException.Create(word);
            }
        }
        throw BdfMissingWordException.Create(WordEndChar);
    }

    private static BdfFont ParseFontSegment(IEnumerator<(string, string)> lines)
    {
        string? name = null;
        int? pointSize = null;
        (int, int)? resolution = null;
        (int, int, int, int)? boundingBox = null;
        BdfProperties? properties = null;
        int? glyphsCount = null;
        List<BdfGlyph> glyphs = [];
        List<string> comments = [];

        Span<int> intValues = stackalloc int[4];
        while (lines.MoveNext())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordFont:
                    name = tail;
                    break;
                case WordSize:
                    ConvertTailToInts(tail, intValues);
                    pointSize = intValues[0];
                    resolution = (intValues[1], intValues[2]);
                    break;
                case WordFontBoundingBox:
                    ConvertTailToInts(tail, intValues);
                    boundingBox = (intValues[0], intValues[1], intValues[2], intValues[3]);
                    break;
                case WordStartProperties:
                    properties = ParsePropertiesSegment(lines, int.Parse(tail));
                    break;
                case WordChars:
                    glyphsCount = int.Parse(tail);
                    glyphs.EnsureCapacity(glyphsCount.Value);
                    break;
                case WordStartChar:
                    glyphs.Add(ParseGlyphSegment(lines, tail));
                    break;
                case WordComment:
                    comments.Add(tail);
                    break;
                case WordEndFont:
                    if (name is null)
                    {
                        throw BdfMissingWordException.Create(WordFont);
                    }
                    if (pointSize is null || resolution is null)
                    {
                        throw BdfMissingWordException.Create(WordSize);
                    }
                    if (boundingBox is null)
                    {
                        throw BdfMissingWordException.Create(WordFontBoundingBox);
                    }
                    if (glyphsCount is null)
                    {
                        throw BdfMissingWordException.Create((WordChars));
                    }
                    if (glyphs.Count != glyphsCount)
                    {
                        throw BdfCountException.Create(WordChars, glyphsCount.Value, glyphs.Count);
                    }
                    return new BdfFont(
                        name,
                        pointSize.Value,
                        resolution.Value,
                        boundingBox.Value,
                        properties,
                        glyphs,
                        comments);
                default:
                    throw BdfIllegalWordException.Create(word);
            }
        }
        throw BdfMissingWordException.Create(WordEndFont);
    }

    public static BdfFont ParseReader(TextReader reader)
    {
        var lines = CreateLinesEnumerator(reader);
        while (lines.MoveNext())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordStartFont:
                    if (tail is not SpecVersion)
                    {
                        throw new BdfParseException($"Spec version not support: {tail}");
                    }
                    return ParseFontSegment(lines);
                default:
                    throw BdfIllegalWordException.Create(word);
            }
        }
        throw BdfMissingWordException.Create(WordStartFont);
    }

    private static void DumpWordStringLine(TextWriter writer, string word, string? tail = null)
    {
        writer.Write(word);
        if (tail is not null)
        {
            tail = tail.Trim();
            if (tail is not "")
            {
                if (tail.IndexOfAny(NewLineChars) >= 0)
                {
                    throw new BdfDumpException("Tail cannot be multi-line string.");
                }
                writer.Write(' ');
                writer.Write(tail);
            }
        }
        writer.Write('\n');
    }

    private static void DumpWordIntsLine(TextWriter writer, string word, int value)
    {
        writer.Write(word);
        writer.Write(' ');
        writer.Write(value);
        writer.Write('\n');
    }

    private static void DumpWordIntsLine(TextWriter writer, string word, int value1, int value2)
    {
        writer.Write(word);
        writer.Write(' ');
        writer.Write(value1);
        writer.Write(' ');
        writer.Write(value2);
        writer.Write('\n');
    }

    private static void DumpWordIntsLine(TextWriter writer, string word, int value1, int value2, int value3)
    {
        writer.Write(word);
        writer.Write(' ');
        writer.Write(value1);
        writer.Write(' ');
        writer.Write(value2);
        writer.Write(' ');
        writer.Write(value3);
        writer.Write('\n');
    }

    private static void DumpWordIntsLine(TextWriter writer, string word, int value1, int value2, int value3, int value4)
    {
        writer.Write(word);
        writer.Write(' ');
        writer.Write(value1);
        writer.Write(' ');
        writer.Write(value2);
        writer.Write(' ');
        writer.Write(value3);
        writer.Write(' ');
        writer.Write(value4);
        writer.Write('\n');
    }

    private static void DumpPropertyLine(TextWriter writer, string key, BdfPropertyValue value)
    {
        writer.Write(key);
        writer.Write(' ');
        if (value.IsString)
        {
            var stringValue = value.AsString();
            if (stringValue.IndexOfAny(NewLineChars) >= 0)
            {
                throw new BdfDumpException("Property value cannot be multi-line string.");
            }

            writer.Write('"');
            writer.Write(stringValue.Replace("\"", "\"\""));
            writer.Write('"');
        }
        else
        {
            writer.Write(value.AsInt());
        }
        writer.Write('\n');
    }

    public static void DumpWriter(TextWriter writer, BdfFont font)
    {
        DumpWordStringLine(writer, WordStartFont, SpecVersion);
        foreach (var comment in font.Comments)
        {
            DumpWordStringLine(writer, WordComment, comment);
        }
        DumpWordStringLine(writer, WordFont, font.Name);
        DumpWordIntsLine(writer, WordSize, font.PointSize, font.ResolutionX, font.ResolutionY);
        DumpWordIntsLine(writer, WordFontBoundingBox, font.Width, font.Height, font.OffsetX, font.OffsetY);

        DumpWordIntsLine(writer, WordStartProperties, font.Properties.Count);
        foreach (var comment in font.Properties.Comments)
        {
            DumpWordStringLine(writer, WordComment, comment);
        }
        foreach (var (key, value) in font.Properties)
        {
            DumpPropertyLine(writer, key, value);
        }
        DumpWordStringLine(writer, WordEndProperties);

        DumpWordIntsLine(writer, WordChars, font.Glyphs.Count);
        foreach (var glyph in font.Glyphs)
        {
            DumpWordStringLine(writer, WordStartChar, glyph.Name);
            foreach (var comment in glyph.Comments)
            {
                DumpWordStringLine(writer, WordComment, comment);
            }
            DumpWordIntsLine(writer, WordEncoding, glyph.Encoding);
            DumpWordIntsLine(writer, WordSWidth, glyph.ScalableWidthX, glyph.ScalableWidthY);
            DumpWordIntsLine(writer, WordDWidth, glyph.DeviceWidthX, glyph.DeviceWidthY);
            DumpWordIntsLine(writer, WordBbx, glyph.Width, glyph.Height, glyph.OffsetX, glyph.OffsetY);
            DumpWordStringLine(writer, WordBitmap);

            var bitmapWidth = (glyph.Width + 7) / 8 * 8;
            foreach (var bitmapRow in glyph.Bitmap)
            {
                var widthLimit = Math.Min(bitmapRow.Count, glyph.Width);
                for (var i = 0; i < bitmapWidth; i += 8)
                {
                    byte b = 0;
                    for (var shift = 0; shift < 8; shift++)
                    {
                        var pixelIndex = i + shift;
                        var pixel = pixelIndex < widthLimit && bitmapRow[pixelIndex] != 0 ? 1 : 0;
                        b = (byte)((b << 1) | pixel);
                    }
                    writer.Write(HexUpperChars[b >> 4]);
                    writer.Write(HexUpperChars[b & 0x0F]);
                }
                writer.Write('\n');
            }

            DumpWordStringLine(writer, WordEndChar);
        }

        DumpWordStringLine(writer, WordEndFont);
    }
}
