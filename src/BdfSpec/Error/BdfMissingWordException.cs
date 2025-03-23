namespace BdfSpec.Error;

public class BdfMissingWordException(string message, string word) : BdfParseException(message)
{
    public static BdfMissingWordException Create(string word)
    {
        var message = $"Missing word: {word}";
        return new BdfMissingWordException(message, word);
    }

    public readonly string Word = word;
}
