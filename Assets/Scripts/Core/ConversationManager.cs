using System;
using System.IO;
using System.Linq;

using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    #region Singleton implementation

    public static ConversationManager Instance;
    private void Awake() => Instance = this;

    #endregion

    #region Events

    public event Action<string> OnMessageReceived;
    public event Action<int> OnTurnChanged;

    #endregion

    public const int TurnCount = 2;
    public static int Turn = 0;
    
    private static string _currentIso;

    public static async void Message(string message)
    {
        // Translating messages
        try
        {
            if (Turn == 0)
            {
                LanguageRecognizer languageRecognizer = new(Path.Combine(Application.streamingAssetsPath, "Other", "Languages", "Core14.profile.xml"));
                
                string iso = languageRecognizer.Identify(message)[..^1];
                
                string newMessage = message;
                
                if (iso != "en") newMessage = await Translator.Translate(iso, "en", message);
                if (newMessage == string.Empty) throw new Exception();
                
                message = newMessage;
                _currentIso = iso;
            }
            else
            {
                string[] parts = message.Split("```");

                string newMessage = string.Empty;
                for (int i = 0; i < parts.Length; i++)
                {
                    string newPart = parts[i];
                    
                    if (i % 2 == 0 && newPart != string.Empty)
                    {
                        if (_currentIso != "en") newPart = await Translator.Translate("en", _currentIso, parts[i]);
                        if (newPart == string.Empty) throw new Exception();
                    }

                    newMessage += newPart;
                }
                
                message = newMessage;
            }
        }
        catch (Exception) {  }

        // Invoking events
        Instance.OnMessageReceived?.Invoke(message);
        Instance.OnTurnChanged?.Invoke(Turn = ++Turn % TurnCount);
    }

    public static void Clear() => Turn = 0;
}
