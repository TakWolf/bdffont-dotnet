namespace BdfSpec.Tests;

public class BdfPropertyValueTests
{
    [Fact]
    public void TestValue()
    {
        {
            var value = new BdfPropertyValue(42);
            Assert.True(value.IsInt);
            Assert.False(value.IsString);
            Assert.Equal(42, value.AsInt());
            Assert.Throws<InvalidOperationException>(() => value.AsString());
        }
        {
            var value = new BdfPropertyValue("hello");
            Assert.True(value.IsString);
            Assert.False(value.IsInt);
            Assert.Equal("hello", value.AsString());
            Assert.Throws<InvalidOperationException>(() => value.AsInt());
        }
        {
            Assert.Throws<ArgumentNullException>(() => new BdfPropertyValue(null!));
        }
        {
            BdfPropertyValue value = default;
            Assert.True(value.IsInt);
            Assert.Equal(0, value.AsInt());
        }
        {
            var value = new BdfPropertyValue(42);
            var (intValue, stringValue) = value;
            Assert.Equal(42, intValue);
            Assert.Null(stringValue);
        }
        {
            var value = new BdfPropertyValue("hello");
            var (intValue, stringValue) = value;
            Assert.Equal("hello", stringValue);
            Assert.Null(intValue);
        }
        {
            BdfPropertyValue value = 42;
            Assert.True(value.IsInt);
            Assert.Equal(42, value.AsInt());
        }
        {
            BdfPropertyValue value = "hello";
            Assert.True(value.IsString);
            Assert.Equal("hello", value.AsString());
        }
        {
            var value = (int)new BdfPropertyValue(42);
            Assert.Equal(42, value);
        }
        {
            var value = (string)new BdfPropertyValue("hello");
            Assert.Equal("hello", value);
        }
        {
            Assert.Throws<InvalidOperationException>(() => (string)new BdfPropertyValue(42));
            Assert.Throws<InvalidOperationException>(() => (int)new BdfPropertyValue("hello"));
        }
    }

    [Fact]
    public void TestEquals()
    {
        {
            var a = new BdfPropertyValue(42);
            var b = new BdfPropertyValue(42);
            Assert.True(a.Equals(b));
            Assert.True(a.Equals((object)b));
            Assert.True(a == b);
            Assert.False(a != b);
        }
        {
            var a = new BdfPropertyValue(1);
            var b = new BdfPropertyValue(42);
            Assert.False(a.Equals(b));
            Assert.False(a.Equals((object)b));
            Assert.False(a == b);
            Assert.True(a != b);
        }
        {
            var a = new BdfPropertyValue("hello");
            var b = new BdfPropertyValue("hello");
            Assert.True(a.Equals(b));
            Assert.True(a.Equals((object)b));
            Assert.True(a == b);
            Assert.False(a != b);
        }
        {
            var a = new BdfPropertyValue("hello");
            var b = new BdfPropertyValue("world");
            Assert.False(a.Equals(b));
            Assert.False(a.Equals((object)b));
            Assert.False(a == b);
            Assert.True(a != b);
        }
        {
            var a = new BdfPropertyValue(42);
            var b = new BdfPropertyValue("hello");
            Assert.False(a.Equals(b));
            Assert.False(a.Equals((object)b));
            Assert.False(a == b);
            Assert.True(a != b);
        }
        {
            var a = new BdfPropertyValue("hello");
            var b = new BdfPropertyValue(42);
            Assert.False(a.Equals(b));
            Assert.False(a.Equals((object)b));
            Assert.False(a == b);
            Assert.True(a != b);
        }
    }

    [Fact]
    public void TestGetHashCode()
    {
        {
            var a = new BdfPropertyValue(42);
            var b = new BdfPropertyValue(42);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }
        {
            var a = new BdfPropertyValue("hello");
            var b = new BdfPropertyValue("hello");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }
        {
            var a = new BdfPropertyValue(42);
            var b = new BdfPropertyValue("hello");
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }

    [Fact]
    public void TestToString()
    {
        {
            var value = new BdfPropertyValue(42);
            Assert.Equal("42", value.ToString());
        }
        {
            var value = new BdfPropertyValue("hello");
            Assert.Equal("hello", value.ToString());
        }
    }
}
