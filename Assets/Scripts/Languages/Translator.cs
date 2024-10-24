using System;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

public static class Translator
{
    public static async Task<string> Translate(string sourceLanguage, string targetLanguage, string sourceText)
    {
        string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLanguage}&tl={targetLanguage}&dt=t&dj=1&q={UnityWebRequest.EscapeURL(sourceText)}";

        UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.SendWebRequest();
        while (!uwr.isDone) await Task.Yield();
        if (uwr.error != null) return string.Empty;

        LanguageSentences languageSentences = JsonUtility.FromJson<LanguageSentences>(uwr.downloadHandler.text);
        StringBuilder translatedText = new();
        foreach (LanguageSentence languageSentence in languageSentences.sentences) translatedText.Append(languageSentence.trans);

        return translatedText.ToString();
    }
}

[Serializable]
public class LanguageSentences
{
    public LanguageSentence[] sentences;
}

[Serializable]
public class LanguageSentence
{
    public string trans;
}