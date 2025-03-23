namespace BdfSpec.Error;

public class BdfIllegalWordException(string message, string word) : BdfParseException(message)
{
    public static BdfIllegalWordException Create(string word)
    {
        var message = $"Illegal word: {word}";
        return new BdfIllegalWordException(message, word);
    }

    public readonly string Word = word;
}
