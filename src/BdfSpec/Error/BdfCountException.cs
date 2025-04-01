namespace BdfSpec.Error;

public class BdfCountException : BdfParseException
{
    public static BdfCountException Create(string word, int expected, int actual)
    {
        var message = $"The count of {word} is incorrect: {expected} -> {actual}";
        return new BdfCountException(message, word, expected, actual);
    }

    public readonly string Word;
    public readonly int Expected;
    public readonly int Actual;

    private BdfCountException(string message, string word, int expected, int actual) : base(message)
    {
        Word = word;
        Expected = expected;
        Actual = actual;
    }
}
