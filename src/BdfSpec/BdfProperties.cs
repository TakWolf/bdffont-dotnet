using System.Collections;
using System.Text.RegularExpressions;
using BdfSpec.Errors;
using BdfSpec.Utils;

namespace BdfSpec;

public partial class BdfProperties : IDictionary<string, BdfPropertyValue>, IList<KeyValuePair<string, BdfPropertyValue>>, ICopyable<BdfProperties>, IEquatable<BdfProperties>
{
    private const string KeyFoundry = "FOUNDRY";
    private const string KeyFamilyName = "FAMILY_NAME";
    private const string KeyWeightName = "WEIGHT_NAME";
    private const string KeySlant = "SLANT";
    private const string KeySetWidthName = "SETWIDTH_NAME";
    private const string KeyAddStyleName = "ADD_STYLE_NAME";
    private const string KeyPixelSize = "PIXEL_SIZE";
    private const string KeyPointSize = "POINT_SIZE";
    private const string KeyResolutionX = "RESOLUTION_X";
    private const string KeyResolutionY = "RESOLUTION_Y";
    private const string KeySpacing = "SPACING";
    private const string KeyAverageWidth = "AVERAGE_WIDTH";
    private const string KeyCharsetRegistry = "CHARSET_REGISTRY";
    private const string KeyCharsetEncoding = "CHARSET_ENCODING";

    private const string KeyDefaultChar = "DEFAULT_CHAR";
    private const string KeyFontAscent = "FONT_ASCENT";
    private const string KeyFontDescent = "FONT_DESCENT";
    private const string KeyXHeight = "X_HEIGHT";
    private const string KeyCapHeight = "CAP_HEIGHT";
    private const string KeyUnderlinePosition = "UNDERLINE_POSITION";
    private const string KeyUnderlineThickness = "UNDERLINE_THICKNESS";

    private const string KeyFontVersion = "FONT_VERSION";
    private const string KeyCopyright = "COPYRIGHT";
    private const string KeyNotice = "NOTICE";

    private static readonly string[] StringValueKeys = [
        KeyFoundry,
        KeyFamilyName,
        KeyWeightName,
        KeySlant,
        KeySetWidthName,
        KeyAddStyleName,
        KeySpacing,
        KeyCharsetRegistry,
        KeyCharsetEncoding,
        KeyFontVersion,
        KeyCopyright,
        KeyNotice
    ];

    private static readonly string[] IntValueKeys = [
        KeyPixelSize,
        KeyPointSize,
        KeyResolutionX,
        KeyResolutionY,
        KeyAverageWidth,
        KeyDefaultChar,
        KeyFontAscent,
        KeyFontDescent,
        KeyXHeight,
        KeyCapHeight,
        KeyUnderlinePosition,
        KeyUnderlineThickness
    ];

    private static readonly string[] XlfdStringValueKeys = [
        KeyFoundry,
        KeyFamilyName,
        KeyWeightName,
        KeySlant,
        KeySetWidthName,
        KeyAddStyleName,
        KeySpacing,
        KeyCharsetRegistry,
        KeyCharsetEncoding
    ];

    private static readonly string[] XlfdKeysOrder = [
        KeyFoundry,
        KeyFamilyName,
        KeyWeightName,
        KeySlant,
        KeySetWidthName,
        KeyAddStyleName,
        KeyPixelSize,
        KeyPointSize,
        KeyResolutionX,
        KeyResolutionY,
        KeySpacing,
        KeyAverageWidth,
        KeyCharsetRegistry,
        KeyCharsetEncoding
    ];

    [GeneratedRegex("^[a-zA-Z0-9_]*$")]
    private static partial Regex RegexPropKey();

    [GeneratedRegex("[-?*,\"]")]
    private static partial Regex RegexXlfdValue();

    private static void CheckKey(string key)
    {
        if (!RegexPropKey().IsMatch(key))
        {
            throw new BdfKeyException("Key contains illegal characters.");
        }
    }

    private static void CheckValue(string key, BdfPropertyValue value)
    {
        if (StringValueKeys.Contains(key) && !value.IsString)
        {
            throw new BdfValueException($"Value of '{key}' must be 'string'.");
        }
        if (IntValueKeys.Contains(key) && !value.IsInt)
        {
            throw new BdfValueException($"Value of '{key}' must be 'int'.");
        }
    }

    private static void CheckXlfdStringValue(string key, string value)
    {
        var match = RegexXlfdValue().Match(value);
        if (match.Success)
        {
            throw new BdfValueException($"Value of '{key}' contains illegal characters '{match.Value}'.");
        }
    }

    private readonly OrderedDictionary<string, BdfPropertyValue> _properties;

    public List<string> Comments { get; set; }

    public BdfProperties(
        IDictionary<string, BdfPropertyValue>? properties = null,
        List<string>? comments = null)
    {
        _properties = new OrderedDictionary<string, BdfPropertyValue>(properties?.Count ?? 0);
        if (properties is not null)
        {
            foreach (var (key, value) in properties)
            {
                this[key] = value;
            }
        }
        Comments = comments ?? [];
    }

    public int Count => _properties.Count;

    bool ICollection<KeyValuePair<string, BdfPropertyValue>>.IsReadOnly => false;

    public ICollection<string> Keys => _properties.Keys;

    public ICollection<BdfPropertyValue> Values => _properties.Values;

    public IEnumerator<KeyValuePair<string, BdfPropertyValue>> GetEnumerator() => _properties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public BdfPropertyValue this[string key]
    {
        get => _properties[key.ToUpper()];
        set
        {
            key = key.ToUpper();
            CheckKey(key);
            CheckValue(key, value);
            _properties[key] = value;
        }
    }

    KeyValuePair<string, BdfPropertyValue> IList<KeyValuePair<string, BdfPropertyValue>>.this[int index]
    {
        get => (_properties as IList<KeyValuePair<string, BdfPropertyValue>>)[index];
        set
        {
            var key = value.Key.ToUpper();
            CheckKey(key);
            CheckValue(key, value.Value);
            (_properties as IList<KeyValuePair<string, BdfPropertyValue>>)[index] = new KeyValuePair<string, BdfPropertyValue>(key, value.Value);
        }
    }

    public bool TryGetValue(string key, out BdfPropertyValue value) => _properties.TryGetValue(key.ToUpper(), out value);

    public bool ContainsKey(string key) => _properties.ContainsKey(key.ToUpper());

    public bool ContainsValue(BdfPropertyValue value) => _properties.ContainsValue(value);

    bool ICollection<KeyValuePair<string, BdfPropertyValue>>.Contains(KeyValuePair<string, BdfPropertyValue> item) => (_properties as ICollection<KeyValuePair<string, BdfPropertyValue>>).Contains(new KeyValuePair<string, BdfPropertyValue>(item.Key.ToUpper(), item.Value));

    int IList<KeyValuePair<string, BdfPropertyValue>>.IndexOf(KeyValuePair<string, BdfPropertyValue> item) => (_properties as IList<KeyValuePair<string, BdfPropertyValue>>).IndexOf(new KeyValuePair<string, BdfPropertyValue>(item.Key.ToUpper(), item.Value));

    public void Add(string key, BdfPropertyValue value)
    {
        key = key.ToUpper();
        CheckKey(key);
        CheckValue(key, value);
        _properties.Add(key, value);
    }

    void ICollection<KeyValuePair<string, BdfPropertyValue>>.Add(KeyValuePair<string, BdfPropertyValue> item)
    {
        var key = item.Key.ToUpper();
        CheckKey(key);
        CheckValue(key, item.Value);
        (_properties as ICollection<KeyValuePair<string, BdfPropertyValue>>).Add(new KeyValuePair<string, BdfPropertyValue>(key, item.Value));
    }

    void IList<KeyValuePair<string, BdfPropertyValue>>.Insert(int index, KeyValuePair<string, BdfPropertyValue> item)
    {
        var key = item.Key.ToUpper();
        CheckKey(key);
        CheckValue(key, item.Value);
        (_properties as IList<KeyValuePair<string, BdfPropertyValue>>).Insert(index, new KeyValuePair<string, BdfPropertyValue>(key, item.Value));
    }

    public bool Remove(string key) => _properties.Remove(key.ToUpper());

    bool ICollection<KeyValuePair<string, BdfPropertyValue>>.Remove(KeyValuePair<string, BdfPropertyValue> item) => (_properties as ICollection<KeyValuePair<string, BdfPropertyValue>>).Remove(new KeyValuePair<string, BdfPropertyValue>(item.Key.ToUpper(), item.Value));

    void IList<KeyValuePair<string, BdfPropertyValue>>.RemoveAt(int index) => (_properties as IList<KeyValuePair<string, BdfPropertyValue>>).RemoveAt(index);

    public void Clear() => _properties.Clear();

    void ICollection<KeyValuePair<string, BdfPropertyValue>>.CopyTo(KeyValuePair<string, BdfPropertyValue>[] array, int arrayIndex) => (_properties as ICollection<KeyValuePair<string, BdfPropertyValue>>).CopyTo(array, arrayIndex);

    public BdfPropertyValue? GetValue(string key) => TryGetValue(key, out var value) ? value : (BdfPropertyValue?)null;

    public string? GetStringValue(string key) => GetValue(key)?.AsString();

    public int? GetIntValue(string key) => GetValue(key)?.AsInt();

    public void SetValue(string key, BdfPropertyValue? value)
    {
        if (value is null)
        {
            Remove(key);
        }
        else
        {
            this[key] = value.Value;
        }
    }

    public void SetStringValue(string key, string? value)
    {
        if (value is null)
        {
            Remove(key);
        }
        else
        {
            this[key] = value;
        }
    }

    public void SetIntValue(string key, int? value)
    {
        if (value is null)
        {
            Remove(key);
        }
        else
        {
            this[key] = value.Value;
        }
    }

    public string? Foundry
    {
        get => GetStringValue(KeyFoundry);
        set => SetStringValue(KeyFoundry, value);
    }

    public string? FamilyName
    {
        get => GetStringValue(KeyFamilyName);
        set => SetStringValue(KeyFamilyName, value);
    }

    public string? WeightName
    {
        get => GetStringValue(KeyWeightName);
        set => SetStringValue(KeyWeightName, value);
    }

    public string? Slant
    {
        get => GetStringValue(KeySlant);
        set => SetStringValue(KeySlant, value);
    }

    public string? SetWidthName
    {
        get => GetStringValue(KeySetWidthName);
        set => SetStringValue(KeySetWidthName, value);
    }

    public string? AddStyleName
    {
        get => GetStringValue(KeyAddStyleName);
        set => SetStringValue(KeyAddStyleName, value);
    }

    public int? PixelSize
    {
        get => GetIntValue(KeyPixelSize);
        set => SetIntValue(KeyPixelSize, value);
    }

    public int? PointSize
    {
        get => GetIntValue(KeyPointSize);
        set => SetIntValue(KeyPointSize, value);
    }

    public int? ResolutionX
    {
        get => GetIntValue(KeyResolutionX);
        set => SetIntValue(KeyResolutionX, value);
    }

    public int? ResolutionY
    {
        get => GetIntValue(KeyResolutionY);
        set => SetIntValue(KeyResolutionY, value);
    }

    public string? Spacing
    {
        get => GetStringValue(KeySpacing);
        set => SetStringValue(KeySpacing, value);
    }

    public int? AverageWidth
    {
        get => GetIntValue(KeyAverageWidth);
        set => SetIntValue(KeyAverageWidth, value);
    }

    public string? CharsetRegistry
    {
        get => GetStringValue(KeyCharsetRegistry);
        set => SetStringValue(KeyCharsetRegistry, value);
    }

    public string? CharsetEncoding
    {
        get => GetStringValue(KeyCharsetEncoding);
        set => SetStringValue(KeyCharsetEncoding, value);
    }

    public int? DefaultChar
    {
        get => GetIntValue(KeyDefaultChar);
        set => SetIntValue(KeyDefaultChar, value);
    }

    public int? FontAscent
    {
        get => GetIntValue(KeyFontAscent);
        set => SetIntValue(KeyFontAscent, value);
    }

    public int? FontDescent
    {
        get => GetIntValue(KeyFontDescent);
        set => SetIntValue(KeyFontDescent, value);
    }

    public int? XHeight
    {
        get => GetIntValue(KeyXHeight);
        set => SetIntValue(KeyXHeight, value);
    }

    public int? CapHeight
    {
        get => GetIntValue(KeyCapHeight);
        set => SetIntValue(KeyCapHeight, value);
    }

    public int? UnderlinePosition
    {
        get => GetIntValue(KeyUnderlinePosition);
        set => SetIntValue(KeyUnderlinePosition, value);
    }

    public int? UnderlineThickness
    {
        get => GetIntValue(KeyUnderlineThickness);
        set => SetIntValue(KeyUnderlineThickness, value);
    }

    public string? FontVersion
    {
        get => GetStringValue(KeyFontVersion);
        set => SetStringValue(KeyFontVersion, value);
    }

    public string? Copyright
    {
        get => GetStringValue(KeyCopyright);
        set => SetStringValue(KeyCopyright, value);
    }

    public string? Notice
    {
        get => GetStringValue(KeyNotice);
        set => SetStringValue(KeyNotice, value);
    }

    public string ToXlfd()
    {
        List<string> parts = [""];
        foreach (var key in XlfdKeysOrder)
        {
            var value = GetValue(key)?.ToString() ?? "";
            if (XlfdStringValueKeys.Contains(key))
            {
                CheckXlfdStringValue(key, value);
            }
            parts.Add(value);
        }
        return string.Join("-", parts);
    }

    public void UpdateByXlfd(string fontName)
    {
        if (!fontName.StartsWith('-'))
        {
            throw new BdfXlfdException("Not starts with '-'.");
        }
        var parts = fontName[1..].Split('-');
        if (parts.Length != 14)
        {
            throw new BdfXlfdException("Must be 14 '-'.");
        }
        foreach (var (key, part) in XlfdKeysOrder.Zip(parts))
        {
            BdfPropertyValue? value;
            if (part is "")
            {
                value = null;
            }
            else
            {
                if (XlfdStringValueKeys.Contains(key))
                {
                    CheckXlfdStringValue(key, part);
                    value = part;
                }
                else
                {
                    value = int.Parse(part);
                }
            }
            SetValue(key, value);
        }
    }

    public BdfProperties Copy() => new(_properties, Comments);

    public BdfProperties DeepCopy() => new(_properties, [.. Comments]);

    public bool Equals(BdfProperties? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return EqualUtil.DictionaryEquals(_properties, other._properties) &&
               EqualUtil.ListEquals(Comments, other.Comments);
    }

    public override bool Equals(object? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (other.GetType() != GetType())
        {
            return false;
        }
        return Equals((BdfProperties)other);
    }

    public override int GetHashCode() => 0;
}
