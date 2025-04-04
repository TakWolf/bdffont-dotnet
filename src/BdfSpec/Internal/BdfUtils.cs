using System.Text;
using System.Text.RegularExpressions;
using BdfSpec.Error;

namespace BdfSpec.Internal;

internal static partial class BdfUtils
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

    [GeneratedRegex(@"\s+")]
    private static partial Regex RegexBlanks();

    [GeneratedRegex(@"(\r\n|\r|\n)")]
    private static partial Regex RegexNewLine();

    private static List<int> ConvertTailToInts(string tail) => RegexBlanks().Split(tail).Select(int.Parse).ToList();

    private static object ConvertTailToPropertiesValue(string tail)
    {
        object value;
        if (tail.StartsWith('"') && tail.EndsWith('"'))
        {
            value = tail[1..^1].Replace("\"\"", "\"");
        }
        else
        {
            try
            {
                value = int.Parse(tail);
            }
            catch (Exception)
            {
                value = tail;
            }
        }
        return value;
    }

    private static List<byte> HexStringToBitmapRow(string hexString, int bitmapWidth)
    {
        hexString = hexString.PadRight(hexString.Length + 1 - (hexString.Length + 1) % 2, '0');
        var bitmapRow = new List<byte>();
        for (var i = 0; i < hexString.Length; i += 2)
        {
            var chunk = hexString[i..(i + 2)];
            bitmapRow.AddRange($"{Convert.ToByte(chunk, 16):b8}".Select(bit => byte.Parse(bit.ToString())));
        }
        if (bitmapRow.Count > bitmapWidth)
        {
            bitmapRow = bitmapRow[..bitmapWidth];
        }
        return bitmapRow;
    }

    private static string BitmapRowToHexString(List<byte> bitmapRow, int bitmapWidth)
    {
        bitmapWidth = bitmapWidth + 7 - (bitmapWidth + 7) % 8;
        var binaryString = string.Join("", bitmapRow);
        if (binaryString.Length < bitmapWidth)
        {
            binaryString = binaryString.PadRight(bitmapWidth, '0');
        }
        else if (binaryString.Length > bitmapWidth)
        {
            binaryString = binaryString[..bitmapWidth];
        }
        var hexString = new StringBuilder();
        for (var i = 0; i < binaryString.Length; i += 8)
        {
            var chunk = binaryString[i..(i + 8)];
            hexString.Append($"{Convert.ToByte(chunk, 2):X2}");
        }
        return hexString.ToString().ToUpper();
    }

    private static IEnumerator<(string, string)> CreateLinesEnumerator(TextReader reader)
    {
        while (reader.ReadLine() is { } line)
        {
            line = line.Trim();
            if (Equals("", line))
            {
                continue;
            }
            var tokens = RegexBlanks().Split(line, 2);
            var word = tokens[0];
            var tail = tokens.Length >= 2 ? tokens[1] : "";
            yield return (word, tail);
        }
    }

    private static BdfProperties ParsePropertiesSegment(IEnumerator<(string, string)> lines, int count)
    {
        var properties = new BdfProperties();
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
                    properties[word] = ConvertTailToPropertiesValue(tail);
                    break;
            }
        }
        throw BdfMissingWordException.Create(WordEndProperties);
    }

    private static List<List<byte>> ParseBitmapSegment(IEnumerator<(string, string)> lines, int bitmapWidth)
    {
        var bitmap = new List<List<byte>>();
        while (lines.MoveNext())
        {
            var (word, _) = lines.Current;
            switch (word)
            {
                case WordEndChar:
                    return bitmap;
                default:
                    bitmap.Add(HexStringToBitmapRow(word, bitmapWidth));
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
        while (lines.MoveNext())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordEncoding:
                    encoding = Convert.ToInt32(tail);
                    break;
                case WordSWidth:
                    var scalableWidthValues = ConvertTailToInts(tail);
                    scalableWidth = (scalableWidthValues[0], scalableWidthValues[1]);
                    break;
                case WordDWidth:
                    var deviceWidthValues = ConvertTailToInts(tail);
                    deviceWidth = (deviceWidthValues[0], deviceWidthValues[1]);
                    break;
                case WordBbx:
                    var boundingBoxValues = ConvertTailToInts(tail);
                    boundingBox = (boundingBoxValues[0], boundingBoxValues[1], boundingBoxValues[2], boundingBoxValues[3]);
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
                    if (Equals(WordBitmap, word))
                    {
                        bitmap = ParseBitmapSegment(lines, boundingBox.Value.Item1);
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
        while (lines.MoveNext())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordFont:
                    name = tail;
                    break;
                case WordSize:
                    var sizeValues = ConvertTailToInts(tail);
                    pointSize = sizeValues[0];
                    resolution = (sizeValues[1], sizeValues[2]);
                    break;
                case WordFontBoundingBox:
                    var boundingBoxValues = ConvertTailToInts(tail);
                    boundingBox = (boundingBoxValues[0], boundingBoxValues[1], boundingBoxValues[2], boundingBoxValues[3]);
                    break;
                case WordStartProperties:
                    properties = ParsePropertiesSegment(lines, Convert.ToInt32(tail));
                    break;
                case WordChars:
                    glyphsCount = Convert.ToInt32(tail);
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
                    if (!Equals(SpecVersion, tail))
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

    private static async IAsyncEnumerator<(string, string)> CreateLinesAsyncEnumerator(TextReader reader)
    {
        while (await reader.ReadLineAsync() is { } line)
        {
            line = line.Trim();
            if (Equals("", line))
            {
                continue;
            }
            var tokens = RegexBlanks().Split(line, 2);
            var word = tokens[0];
            var tail = tokens.Length >= 2 ? tokens[1] : "";
            yield return (word, tail);
        }
    }

    private static async Task<BdfProperties> ParsePropertiesSegmentAsync(IAsyncEnumerator<(string, string)> lines, int count)
    {
        var properties = new BdfProperties();
        while (await lines.MoveNextAsync())
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
                    properties[word] = ConvertTailToPropertiesValue(tail);
                    break;
            }
        }
        throw BdfMissingWordException.Create(WordEndProperties);
    }

    private static async Task<List<List<byte>>> ParseBitmapSegmentAsync(IAsyncEnumerator<(string, string)> lines, int bitmapWidth)
    {
        List<List<byte>> bitmap = [];
        while (await lines.MoveNextAsync())
        {
            var (word, _) = lines.Current;
            switch (word)
            {
                case WordEndChar:
                    return bitmap;
                default:
                    bitmap.Add(HexStringToBitmapRow(word, bitmapWidth));
                    break;
            }
        }
        throw BdfMissingWordException.Create(WordEndChar);
    }

    private static async Task<BdfGlyph> ParseGlyphSegmentAsync(IAsyncEnumerator<(string, string)> lines, string name)
    {
        int? encoding = null;
        (int, int)? scalableWidth = null;
        (int, int)? deviceWidth = null;
        (int, int, int, int)? boundingBox = null;
        List<string> comments = [];
        while (await lines.MoveNextAsync())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordEncoding:
                    encoding = Convert.ToInt32(tail);
                    break;
                case WordSWidth:
                    var scalableWidthValues = ConvertTailToInts(tail);
                    scalableWidth = (scalableWidthValues[0], scalableWidthValues[1]);
                    break;
                case WordDWidth:
                    var deviceWidthValues = ConvertTailToInts(tail);
                    deviceWidth = (deviceWidthValues[0], deviceWidthValues[1]);
                    break;
                case WordBbx:
                    var boundingBoxValues = ConvertTailToInts(tail);
                    boundingBox = (boundingBoxValues[0], boundingBoxValues[1], boundingBoxValues[2], boundingBoxValues[3]);
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
                    if (Equals(WordBitmap, word))
                    {
                        bitmap = await ParseBitmapSegmentAsync(lines, boundingBox.Value.Item1);
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

    private static async Task<BdfFont> ParseFontSegmentAsync(IAsyncEnumerator<(string, string)> lines)
    {
        string? name = null;
        int? pointSize = null;
        (int, int)? resolution = null;
        (int, int, int, int)? boundingBox = null;
        BdfProperties? properties = null;
        int? glyphsCount = null;
        List<BdfGlyph> glyphs = [];
        List<string> comments = [];
        while (await lines.MoveNextAsync())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordFont:
                    name = tail;
                    break;
                case WordSize:
                    var sizeValues = ConvertTailToInts(tail);
                    pointSize = sizeValues[0];
                    resolution = (sizeValues[1], sizeValues[2]);
                    break;
                case WordFontBoundingBox:
                    var boundingBoxValues = ConvertTailToInts(tail);
                    boundingBox = (boundingBoxValues[0], boundingBoxValues[1], boundingBoxValues[2], boundingBoxValues[3]);
                    break;
                case WordStartProperties:
                    properties = await ParsePropertiesSegmentAsync(lines, Convert.ToInt32(tail));
                    break;
                case WordChars:
                    glyphsCount = Convert.ToInt32(tail);
                    break;
                case WordStartChar:
                    glyphs.Add(await ParseGlyphSegmentAsync(lines, tail));
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

    public static async Task<BdfFont> ParseReaderAsync(TextReader reader)
    {
        var lines = CreateLinesAsyncEnumerator(reader);
        while (await lines.MoveNextAsync())
        {
            var (word, tail) = lines.Current;
            switch (word)
            {
                case WordStartFont:
                    if (!Equals(SpecVersion, tail))
                    {
                        throw new BdfParseException($"Spec version not support: {tail}");
                    }
                    return await ParseFontSegmentAsync(lines);
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
            if (!Equals("", tail))
            {
                if (RegexNewLine().IsMatch(tail))
                {
                    throw new BdfDumpException("Tail cannot be multi-line string.");
                }
                writer.Write($" {tail}");
            }
        }
        writer.Write('\n');
    }

    private static void DumpWordIntsLine(TextWriter writer, string word, params int[] values)
    {
        writer.Write(word);
        foreach (var value in values)
        {
            writer.Write($" {value}");
        }
        writer.Write('\n');
    }

    private static void DumpPropertiesLine(TextWriter writer, string key, object value)
    {
        if (value is string stringValue)
        {
            stringValue = stringValue.Replace("\"", "\"\"");
            stringValue = $"\"{stringValue}\"";
            if (RegexNewLine().IsMatch(stringValue))
            {
                throw new BdfDumpException("Properties value cannot be multi-line string.");
            }
            value = stringValue;
        }
        writer.Write($"{key} {value}\n");
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
            DumpPropertiesLine(writer, key, value);
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

            foreach (var bitmapRow in glyph.Bitmap)
            {
                writer.Write($"{BitmapRowToHexString(bitmapRow, glyph.Width)}\n");
            }

            DumpWordStringLine(writer, WordEndChar);
        }

        DumpWordStringLine(writer, WordEndFont);
    }

    private static async Task DumpWordStringLineAsync(TextWriter writer, string word, string? tail = null)
    {
        await writer.WriteAsync(word);
        if (tail is not null)
        {
            tail = tail.Trim();
            if (!Equals("", tail))
            {
                if (RegexNewLine().IsMatch(tail))
                {
                    throw new BdfDumpException("Tail cannot be multi-line string.");
                }
                await writer.WriteAsync($" {tail}");
            }
        }
        await writer.WriteAsync('\n');
    }

    private static async Task DumpWordIntsLineAsync(TextWriter writer, string word, params int[] values)
    {
        await writer.WriteAsync(word);
        foreach (var value in values)
        {
            await writer.WriteAsync($" {value}");
        }
        await writer.WriteAsync('\n');
    }

    private static async Task DumpPropertiesLineAsync(TextWriter writer, string key, object value)
    {
        if (value is string stringValue)
        {
            stringValue = stringValue.Replace("\"", "\"\"");
            stringValue = $"\"{stringValue}\"";
            if (RegexNewLine().IsMatch(stringValue))
            {
                throw new BdfDumpException("Properties value cannot be multi-line string.");
            }
            value = stringValue;
        }
        await writer.WriteAsync($"{key} {value}\n");
    }

    public static async Task DumpWriterAsync(TextWriter writer, BdfFont font)
    {
        await DumpWordStringLineAsync(writer, WordStartFont, SpecVersion);
        foreach (var comment in font.Comments)
        {
            await DumpWordStringLineAsync(writer, WordComment, comment);
        }
        await DumpWordStringLineAsync(writer, WordFont, font.Name);
        await DumpWordIntsLineAsync(writer, WordSize, font.PointSize, font.ResolutionX, font.ResolutionY);
        await DumpWordIntsLineAsync(writer, WordFontBoundingBox, font.Width, font.Height, font.OffsetX, font.OffsetY);

        await DumpWordIntsLineAsync(writer, WordStartProperties, font.Properties.Count);
        foreach (var comment in font.Properties.Comments)
        {
            await DumpWordStringLineAsync(writer, WordComment, comment);
        }
        foreach (var (key, value) in font.Properties)
        {
            await DumpPropertiesLineAsync(writer, key, value);
        }
        await DumpWordStringLineAsync(writer, WordEndProperties);

        await DumpWordIntsLineAsync(writer, WordChars, font.Glyphs.Count);
        foreach (var glyph in font.Glyphs)
        {
            await DumpWordStringLineAsync(writer, WordStartChar, glyph.Name);
            foreach (var comment in glyph.Comments)
            {
                await DumpWordStringLineAsync(writer, WordComment, comment);
            }
            await DumpWordIntsLineAsync(writer, WordEncoding, glyph.Encoding);
            await DumpWordIntsLineAsync(writer, WordSWidth, glyph.ScalableWidthX, glyph.ScalableWidthY);
            await DumpWordIntsLineAsync(writer, WordDWidth, glyph.DeviceWidthX, glyph.DeviceWidthY);
            await DumpWordIntsLineAsync(writer, WordBbx, glyph.Width, glyph.Height, glyph.OffsetX, glyph.OffsetY);
            await DumpWordStringLineAsync(writer, WordBitmap);

            foreach (var bitmapRow in glyph.Bitmap)
            {
                await writer.WriteAsync($"{BitmapRowToHexString(bitmapRow, glyph.Width)}\n");
            }

            await DumpWordStringLineAsync(writer, WordEndChar);
        }

        await DumpWordStringLineAsync(writer, WordEndFont);
    }
}
