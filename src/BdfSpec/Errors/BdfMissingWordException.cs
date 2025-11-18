namespace BdfSpec.Errors;

public class BdfMissingWordException : BdfParseException
{
    public static BdfMissingWordException Create(string word)
    {
        var message = $"Missing word: {word}";
        return new BdfMissingWordException(message, word);
    }

    public readonly string Word;

    private BdfMissingWordException(string message, string word) : base(message)
    {
        Word = word;
    }
}
