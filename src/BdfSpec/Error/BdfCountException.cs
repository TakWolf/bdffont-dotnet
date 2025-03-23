namespace BdfSpec.Error;

public class BdfCountException(string message, string word, int expected, int actual) : BdfParseException(message)
{
    public static BdfCountException Create(string word, int expected, int actual)
    {
        var message = $"The count of {word} is incorrect: {expected} -> {actual}";
        return new BdfCountException(message, word, expected, actual);
    }
    
    public readonly string Word = word;
    public readonly int Expected = expected;
    public readonly int Actual = actual;
}
