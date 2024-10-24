using System.Globalization;
using System.Linq;

using NTextCat;

public class LanguageRecognizer
{
    private readonly RankedLanguageIdentifierFactory _factory;
    private readonly RankedLanguageIdentifier _identifier;
    
    public LanguageRecognizer(string filePath)
    {
        _factory = new RankedLanguageIdentifierFactory();
        _identifier = _factory.Load(filePath);
    }
    
    public string Identify(string inputText)
    {
        var languages = _identifier.Identify(inputText);
        var mostCertainLanguage = languages.FirstOrDefault();

        if (mostCertainLanguage != null && mostCertainLanguage.Item1 != null) return mostCertainLanguage.Item1.Iso639_3;

        return "ukn";
    }
}
