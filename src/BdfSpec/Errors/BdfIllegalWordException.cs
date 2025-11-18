namespace BdfSpec.Errors;

public class BdfIllegalWordException : BdfParseException
{
    public static BdfIllegalWordException Create(string word)
    {
        var message = $"Illegal word: {word}";
        return new BdfIllegalWordException(message, word);
    }

    public readonly string Word;

    private BdfIllegalWordException(string message, string word) : base(message)
    {
        Word = word;
    }
}
