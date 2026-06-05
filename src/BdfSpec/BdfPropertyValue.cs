namespace BdfSpec;

public readonly struct BdfPropertyValue : IEquatable<BdfPropertyValue>
{
    private readonly int _intValue;
    private readonly string? _stringValue;

    public BdfPropertyValue(int value)
    {
        _intValue = value;
        _stringValue = null;
    }

    public BdfPropertyValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _intValue = 0;
        _stringValue = value;
    }

    public bool IsInt => _stringValue is null;

    public bool IsString => _stringValue is not null;

    public int AsInt() => IsInt ? _intValue : throw new InvalidOperationException("The value is not int.");

    public string AsString() => IsString ? _stringValue! : throw new InvalidOperationException("The value is not string.");

    public static implicit operator BdfPropertyValue(int value) => new(value);

    public static implicit operator BdfPropertyValue(string value) => new(value);

    public static explicit operator int(BdfPropertyValue value) => value.AsInt();

    public static explicit operator string(BdfPropertyValue value) => value.AsString();

    public void Deconstruct(out int? intValue, out string? stringValue)
    {
        if (IsInt)
        {
            intValue = _intValue;
            stringValue = null;
        }
        else
        {
            intValue = null;
            stringValue = _stringValue;
        }
    }

    public bool Equals(BdfPropertyValue other) => _intValue == other._intValue && _stringValue == other._stringValue;

    public override bool Equals(object? obj) => obj is BdfPropertyValue other && Equals(other);

    public static bool operator ==(BdfPropertyValue left, BdfPropertyValue right) => left.Equals(right);

    public static bool operator !=(BdfPropertyValue left, BdfPropertyValue right) => !(left == right);

    public override int GetHashCode() => IsInt ? HashCode.Combine(1, _intValue) : HashCode.Combine(2, _stringValue);

    public override string ToString() => IsInt ? _intValue.ToString() : _stringValue!;
}
