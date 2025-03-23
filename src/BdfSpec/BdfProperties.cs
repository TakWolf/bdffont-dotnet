using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using BdfSpec.Error;

namespace BdfSpec;

public partial class BdfProperties : IDictionary<string, object>, IReadOnlyDictionary<string, object>, IDictionary, IList<KeyValuePair<string, object>>, IReadOnlyList<KeyValuePair<string, object>>, IList
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
    private const string KeyCapHeight = "CAP_HEIGHT";
    private const string KeyXHeight = "X_HEIGHT";

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
        KeyCapHeight,
        KeyXHeight
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
        KeyCharsetEncoding,
    ];
    
    [GeneratedRegex("^[a-zA-Z0-9_]*$")]
    private static partial Regex RegexPropKey();
    
    [GeneratedRegex("[-?*,\"]")]
    private static partial Regex RegexXlfdValue();

    private static void CheckKey(string key)
    {
        if (!RegexPropKey().IsMatch(key))
        {
            throw new BdfKeyException("Contains illegal characters.");
        }
    }
    
    private static void CheckValue(string key, object value)
    {
        if (StringValueKeys.Contains(key))
        {
            if (value is not string)
            {
                throw new BdfValueException($"Expected type 'string', got '{value.GetType()}' instead.");
            }
        }
        else if (IntValueKeys.Contains(key))
        {
            if (value is not int)
            {
                throw new BdfValueException($"Expected type 'int', got '{value.GetType()}' instead.");
            }
        }
        else
        {
            if (value is not string && value is not int)
            {
                throw new BdfValueException($"Expected type 'string' or 'int', got '{value.GetType()}' instead.");
            }
        }

        if (XlfdStringValueKeys.Contains(key))
        {
            if (RegexXlfdValue().IsMatch((string) value))
            {
                throw new BdfValueException("Contains illegal characters.");
            }
        }   
    }

    private static BdfKeyException CreateKeyNotFoundException(string key) => new($"The given key '{key}' was not present in the dictionary.");

    private static BdfKeyException CreateKeyNotStringException(object key) => new($"Expected type 'string', got '{key.GetType()}' instead.");
 
    private static BdfKeyException CreateKeyAlreadyExistsException(string key) => new($"An element with the same key '{key}' already exists.");

    private static ArgumentException CreateItemNotPairException(object item) => new($"Expected type '{typeof(KeyValuePair<string, object>)}', got '{item.GetType()}' instead.", nameof(item));

    private readonly List<string> _keysData = [];
    private readonly List<object> _valuesData = [];
    private KeyCollection? _keys;
    private ValueCollection? _values;
    
    public List<string> Comments;
    
    public BdfProperties(
        IDictionary<string, object>? data = null,
        List<string>? comments = null)
    {
        if (data is not null)
        {
            foreach (var pair in data)
            {
                this[pair.Key] = pair.Value;
            }
        }
        Comments = comments ?? [];
    }
    
    public int Count => _keysData.Count;

    bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;
    
    bool IDictionary.IsReadOnly => false;

    bool IList.IsReadOnly => false;
    
    bool IDictionary.IsFixedSize => false;
    
    bool IList.IsFixedSize => false;
    
    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;
    
    public object? GetValue(string key)
    {
        var index = _keysData.IndexOf(key.ToUpper());
        return index >= 0 ? _valuesData[index] : null;
    }

    public string? GetStringValue(string key) => (string?) GetValue(key);
    
    public int? GetIntValue(string key) => (int?) GetValue(key);
    
    public void SetValue(string key, object? value)
    {
        key = key.ToUpper();
        CheckKey(key);
        var index = _keysData.IndexOf(key);
        
        if (value is null)
        {
            if (index >= 0)
            {
                _keysData.RemoveAt(index);
                _valuesData.RemoveAt(index);
            }
            return;
        }
        
        CheckValue(key, value);
        if (index >= 0)
        {
            _valuesData[index] = value;
        }
        else
        {
            _keysData.Add(key);
            _valuesData.Add(value);
        }
    }
    
    public string GetKeyAt(int index) => _keysData[index];

    public object GetValueAt(int index) => _valuesData[index];

    public KeyValuePair<string, object> GetAt(int index) => new(GetKeyAt(index), GetValueAt(index));

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
    {
        var index = _keysData.IndexOf(key.ToUpper());
        if (index >= 0)
        {
            value = _valuesData[index];
            return true;
        }
        value = null;
        return false;
    }
    
    bool IReadOnlyDictionary<string, object>.TryGetValue(string key, [MaybeNullWhen(false)] out object value) => TryGetValue(key, out value);
    
    public void SetAt(int index, object? value)
    {
        if (value is null)
        {
            _keysData.RemoveAt(index);
            _valuesData.RemoveAt(index);
            return;
        }
        
        var key = _keysData[index];
        CheckValue(key, value);
        _valuesData[index] = value;
    }
    
    public void SetAt(int index, string key, object value)
    {
        key = key.ToUpper();
        if (_keysData.IndexOf(key) != index)
        {
            CheckKey(key);
        }
        CheckValue(key, value);
        _keysData[index] = key;
        _valuesData[index] = value;
    }

    public void SetAt(int index, KeyValuePair<string, object> pair) => SetAt(index, pair.Key, pair.Value);

    public object this[string key]
    {
        get => GetValue(key) ?? throw CreateKeyNotFoundException(key);
        set => SetValue(key, value);
    }

    object IReadOnlyDictionary<string, object>.this[string key] => GetValue(key) ?? throw CreateKeyNotFoundException(key);
    
    object? IDictionary.this[object key]
    {
        get => key is string stringKey ? GetValue(stringKey) : null;
        set
        {
            if (key is not string stringKey)
            {
                throw CreateKeyNotStringException(key);
            }
            SetValue(stringKey, value);
        }
    }
    
    KeyValuePair<string, object> IList<KeyValuePair<string, object>>.this[int index]
    {
        get => GetAt(index);
        set => SetAt(index, value);
    }

    KeyValuePair<string, object> IReadOnlyList<KeyValuePair<string, object>>.this[int index] => GetAt(index);
    
    object? IList.this[int index]
    {
        get => GetAt(index);
        set
        {
            switch (value)
            {
                case null:
                    SetAt(index, null);
                    break;
                case KeyValuePair<string, object> pair:
                    SetAt(index, pair);
                    break;
                default:
                    throw CreateItemNotPairException(value);
            }
        }
    }
    
    public void Add(string key, object value)
    {
        key = key.ToUpper();
        if (_keysData.Contains(key))
        {
            throw CreateKeyAlreadyExistsException(key);
        }
        
        CheckKey(key);
        CheckValue(key, value);
        _keysData.Add(key);
        _valuesData.Add(value);
    }
    
    public void Add(KeyValuePair<string, object> pair) => Add(pair.Key, pair.Value);

    void IDictionary.Add(object key, object? value)
    {
        if (value is null)
        {
            return;
        }
        if (key is not string stringKey)
        {
            throw CreateKeyNotStringException(key);
        }
        Add(stringKey, value);
    }
    
    int IList.Add(object? item)
    {
        switch (item)
        {
            case null:
                return -1;
            case KeyValuePair<string, object> pair:
                Add(pair);
                return Count - 1;
            default:
                throw CreateItemNotPairException(item);
        }
    }

    public void Insert(int index, string key, object value)
    {
        key = key.ToUpper();
        if (_keysData.Contains(key))
        {
            throw CreateKeyAlreadyExistsException(key);
        }
        
        CheckKey(key);
        CheckValue(key, value);
        _keysData.Insert(index, key);
        _valuesData.Insert(index, value);
    }
    
    public void Insert(int index, KeyValuePair<string, object> pair) => Insert(index, pair.Key, pair.Value);
    
    void IList.Insert(int index, object? item)
    {
        switch (item)
        {
            case null:
                return;
            case KeyValuePair<string, object> pair:
                Insert(index, pair);
                break;
            default:
                throw CreateItemNotPairException(item);
        }
    }
    
    public bool Remove(string key)
    {
        var index = _keysData.IndexOf(key.ToUpper());
        if (index < 0)
        {
            return false;
        }
        _keysData.RemoveAt(index);
        _valuesData.RemoveAt(index);
        return true;
    }

    public bool Remove(string key, object value)
    {
        var index = _keysData.IndexOf(key.ToUpper());
        if (index < 0)
        {
            return false;
        }
        if (!Equals(_valuesData[index], value))
        {
            return false;
        }
        _keysData.RemoveAt(index);
        _valuesData.RemoveAt(index);
        return true;
    }

    public bool Remove(KeyValuePair<string, object> pair) => Remove(pair.Key, pair.Value);

    void IDictionary.Remove(object key)
    {
        if (key is not string stringKey)
        {
            throw CreateKeyNotStringException(key);
        }
        Remove(stringKey);
    }
    
    void IList.Remove(object? item)
    {
        if (item is KeyValuePair<string, object> pair)
        {
            Remove(pair);
        }
    }
    
    public void RemoveAt(int index)
    {
        _keysData.RemoveAt(index);
        _valuesData.RemoveAt(index);
    }
    
    public void Clear()
    {
        _keysData.Clear();
        _valuesData.Clear();
    }
    
    public bool ContainsKey(string key) => _keysData.Contains(key.ToUpper());

    public bool ContainsValue(object value) => _valuesData.Contains(value);
    
    public bool ContainsPair(string key, object value) => Equals(GetValue(key), value); 
    
    public bool ContainsPair(KeyValuePair<string, object> pair) => ContainsPair(pair.Key, pair.Value); 
    
    bool IReadOnlyDictionary<string, object>.ContainsKey(string key) => ContainsKey(key);

    bool IDictionary.Contains(object key) => key is string stringKey && ContainsKey(stringKey);
    
    bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> pair) => ContainsPair(pair);

    bool IList.Contains(object? item) => item is KeyValuePair<string, object> pair && ContainsPair(pair);

    public int IndexOfKey(string key) => _keysData.IndexOf(key.ToUpper());

    public int IndexOfValue(object value) => _valuesData.IndexOf(value);

    public int IndexOfPair(string key, object value)
    {
        var index = IndexOfKey(key);
        return Equals(_valuesData[index], value) ? index : -1;
    }

    public int IndexOfPair(KeyValuePair<string, object> pair) => IndexOfPair(pair.Key, pair.Value);
    
    int IList<KeyValuePair<string, object>>.IndexOf(KeyValuePair<string, object> pair) => IndexOfPair(pair);

    int IList.IndexOf(object? item) => item is KeyValuePair<string, object> pair ? IndexOfPair(pair) : -1;
    
    public void CopyTo(KeyValuePair<string, object>[] array, int index)
    {
        foreach (var pair in this)
        {
            array[index++] = pair;
        }
    }

    void ICollection.CopyTo(Array array, int index)
    {
        if (array.Rank != 1)
        {
            throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));
        }
        if (array.GetLowerBound(0) != 0)
        {
            throw new ArgumentException("The lower bound of target array must be zero.", nameof(array));
        }
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"Index ('{index}') must be a non-negative value.");
        }
        if (array.Length - index < Count)
        {
            throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
        }
        if (array is not KeyValuePair<string, object>[] pairArray)
        {
            throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", nameof(array));
        }
        CopyTo(pairArray, index);
    }
    
    public KeyCollection Keys => _keys ??= new KeyCollection(this);
    
    IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => Keys;

    ICollection<string> IDictionary<string, object>.Keys => Keys;

    ICollection IDictionary.Keys => Keys;

    public ValueCollection Values => _values ??= new ValueCollection(this);
    
    IEnumerable<object> IReadOnlyDictionary<string, object>.Values => Values;

    ICollection<object> IDictionary<string, object>.Values => Values;

    ICollection IDictionary.Values => Values;

    public Enumerator GetEnumerator() => new(this);
    
    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IDictionaryEnumerator IDictionary.GetEnumerator() => GetEnumerator();
    
    public string? Foundry
    {
        get => GetStringValue(KeyFoundry);
        set => SetValue(KeyFoundry, value);
    }

    public string? FamilyName
    {
        get => GetStringValue(KeyFamilyName);
        set => SetValue(KeyFamilyName, value);
    }

    public string? WeightName
    {
        get => GetStringValue(KeyWeightName);
        set => SetValue(KeyWeightName, value);
    }

    public string? Slant
    {
        get => GetStringValue(KeySlant);
        set => SetValue(KeySlant, value);
    }

    public string? SetWidthName
    {
        get => GetStringValue(KeySetWidthName);
        set => SetValue(KeySetWidthName, value);
    }

    public string? AddStyleName
    {
        get => GetStringValue(KeyAddStyleName);
        set => SetValue(KeyAddStyleName, value);
    }

    public int? PixelSize
    {
        get => GetIntValue(KeyPixelSize);
        set => SetValue(KeyPixelSize, value);
    }

    public int? PointSize
    {
        get => GetIntValue(KeyPointSize);
        set => SetValue(KeyPointSize, value);
    }

    public int? ResolutionX
    {
        get => GetIntValue(KeyResolutionX);
        set => SetValue(KeyResolutionX, value);
    }

    public int? ResolutionY
    {
        get => GetIntValue(KeyResolutionY);
        set => SetValue(KeyResolutionY, value);
    }

    public string? Spacing
    {
        get => GetStringValue(KeySpacing);
        set => SetValue(KeySpacing, value);
    }

    public int? AverageWidth
    {
        get => GetIntValue(KeyAverageWidth);
        set => SetValue(KeyAverageWidth, value);
    }

    public string? CharsetRegistry
    {
        get => GetStringValue(KeyCharsetRegistry);
        set => SetValue(KeyCharsetRegistry, value);
    }
    
    public string? CharsetEncoding
    {
        get => GetStringValue(KeyCharsetEncoding);
        set => SetValue(KeyCharsetEncoding, value);
    }

    public int? DefaultChar
    {
        get => GetIntValue(KeyDefaultChar);
        set => SetValue(KeyDefaultChar, value);
    }

    public int? FontAscent
    {
        get => GetIntValue(KeyFontAscent);
        set => SetValue(KeyFontAscent, value);
    }

    public int? FontDescent
    {
        get => GetIntValue(KeyFontDescent);
        set => SetValue(KeyFontDescent, value);
    }

    public int? XHeight
    {
        get => GetIntValue(KeyXHeight);
        set => SetValue(KeyXHeight, value);
    }

    public int? CapHeight
    {
        get => GetIntValue(KeyCapHeight);
        set => SetValue(KeyCapHeight, value);
    }

    public string? FontVersion
    {
        get => GetStringValue(KeyFontVersion);
        set => SetValue(KeyFontVersion, value);
    }

    public string? Copyright
    {
        get => GetStringValue(KeyCopyright);
        set => SetValue(KeyCopyright, value);
    }

    public string? Notice
    {
        get => GetStringValue(KeyNotice);
        set => SetValue(KeyNotice, value);
    }

    public string ToXlfd()
    {
        List<string> tokens = [""];
        tokens.AddRange(XlfdKeysOrder.Select(key => GetValue(key)?.ToString() ?? ""));
        return string.Join("-", tokens);
    }

    public void UpdateByXlfd(string fontName)
    {
        if (!fontName.StartsWith('-'))
        {
            throw new BdfXlfdException("Not starts with '-'.");
        }
        var tokens = fontName[1..fontName.Length].Split('-');
        if (tokens.Length != 14)
        {
            throw new BdfXlfdException("Must be 14 '-'.");
        }
        foreach (var (key, token) in XlfdKeysOrder.Zip(tokens))
        {
            object? value;
            if (Equals(token, ""))
            {
                value = null;
            }
            else
            {
                if (XlfdStringValueKeys.Contains(key))
                {
                    value = token;
                }
                else
                {
                    value = Convert.ToInt32(token);
                }
            }
            SetValue(key, value);
        }
    }
    
    public sealed class KeyCollection : IList<string>, IReadOnlyList<string>, IList
    {
        private readonly BdfProperties _properties;

        internal KeyCollection(BdfProperties properties)
        {
            _properties = properties;
        }

        public int Count => _properties.Count;

        bool ICollection<string>.IsReadOnly => true;
        
        bool IList.IsReadOnly => true;
        
        bool IList.IsFixedSize => false;
        
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => (_properties as ICollection).SyncRoot;
        
        string IList<string>.this[int index]
        {
            get => _properties.GetKeyAt(index);
            set => throw new NotSupportedException();
        }
        
        object? IList.this[int index]
        {
            get => _properties.GetKeyAt(index);
            set => throw new NotSupportedException();
        }

        string IReadOnlyList<string>.this[int index] => _properties.GetKeyAt(index);
        
        public bool Contains(string key) => _properties.ContainsKey(key);

        bool IList.Contains(object? key) => key is string stringKey && Contains(stringKey);
        
        public int IndexOf(string key) => _properties.IndexOfKey(key);

        int IList.IndexOf(object? key) => key is string stringKey ? IndexOf(stringKey) : -1;
        
        public void CopyTo(string[] array, int index) => _properties._keysData.CopyTo(array, index);

        void ICollection.CopyTo(Array array, int index) => (_properties._keysData as ICollection).CopyTo(array, index);
        
        public IEnumerator<string> GetEnumerator() => _properties._keysData.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        void ICollection<string>.Add(string key) => throw new NotSupportedException();
        
        int IList.Add(object? item) => throw new NotSupportedException();
        
        void IList<string>.Insert(int index, string key) => throw new NotSupportedException();

        void IList.Insert(int index, object? item) => throw new NotSupportedException();
        
        bool ICollection<string>.Remove(string key) => throw new NotSupportedException();

        void IList.Remove(object? item) => throw new NotSupportedException();
        
        void IList<string>.RemoveAt(int index) => throw new NotSupportedException();
        
        void IList.RemoveAt(int index) => throw new NotSupportedException();
        
        void ICollection<string>.Clear() => throw new NotSupportedException();
        
        void IList.Clear() => throw new NotSupportedException();
    }
    
    public sealed class ValueCollection : IList<object>, IReadOnlyList<object>, IList
    {
        private readonly BdfProperties _properties;

        internal ValueCollection(BdfProperties properties)
        {
            _properties = properties;
        }

        public int Count => _properties.Count;

        bool ICollection<Object>.IsReadOnly => true;
        
        bool IList.IsReadOnly => true;
        
        bool IList.IsFixedSize => false;
        
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => (_properties as ICollection).SyncRoot;
        
        object IList<object>.this[int index]
        {
            get => _properties.GetValueAt(index);
            set => throw new NotSupportedException();
        }
        
        object? IList.this[int index]
        {
            get => _properties.GetValueAt(index);
            set => throw new NotSupportedException();
        }

        object IReadOnlyList<object>.this[int index] => _properties.GetValueAt(index);
        
        public bool Contains(object value) => _properties.ContainsValue(value);

        bool IList.Contains(object? value) => value is not null && Contains(value);
        
        public int IndexOf(object value) => _properties.IndexOfValue(value);

        int IList.IndexOf(object? value) => value is not null ? IndexOf(value) : -1;
        
        public void CopyTo(object[] array, int index) => _properties._valuesData.CopyTo(array, index);

        void ICollection.CopyTo(Array array, int index) => (_properties._valuesData as ICollection).CopyTo(array, index);
        
        public IEnumerator<object> GetEnumerator() => _properties._valuesData.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        void ICollection<object>.Add(object value) => throw new NotSupportedException();
        
        int IList.Add(object? item) => throw new NotSupportedException();
        
        void IList<object>.Insert(int index, object value) => throw new NotSupportedException();

        void IList.Insert(int index, object? item) => throw new NotSupportedException();
        
        bool ICollection<object>.Remove(object value) => throw new NotSupportedException();

        void IList.Remove(object? item) => throw new NotSupportedException();
        
        void IList<object>.RemoveAt(int index) => throw new NotSupportedException();
        
        void IList.RemoveAt(int index) => throw new NotSupportedException();
        
        void ICollection<object>.Clear() => throw new NotSupportedException();
        
        void IList.Clear() => throw new NotSupportedException();
    }

    public readonly struct Enumerator : IEnumerator<KeyValuePair<string, object>>, IDictionaryEnumerator
    {
        private static IEnumerator<KeyValuePair<string, object>> CreateIEnumerator(BdfProperties properties)
        {
            foreach (var (key, value) in properties._keysData.Zip(properties._valuesData))
            {
                yield return new KeyValuePair<string, object>(key, value);
            }
        }

        private readonly IEnumerator<KeyValuePair<string, object>> _enumerator;

        internal Enumerator(BdfProperties properties)
        {
            _enumerator = CreateIEnumerator(properties);
        }

        public KeyValuePair<string, object> Current => _enumerator.Current;

        object IEnumerator.Current => Current;

        DictionaryEntry IDictionaryEnumerator.Entry => new(Current.Key, Current.Value);

        object IDictionaryEnumerator.Key => Current.Key;

        object IDictionaryEnumerator.Value => Current.Value;
        
        public bool MoveNext() => _enumerator.MoveNext();

        void IEnumerator.Reset() => _enumerator.Reset();

        void IDisposable.Dispose() => _enumerator.Dispose();
    }
}
