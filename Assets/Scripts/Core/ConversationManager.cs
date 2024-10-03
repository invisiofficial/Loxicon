using System;

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
    
    public static void Message(string message)
    {
        Instance.OnMessageReceived?.Invoke(message);
        Instance.OnTurnChanged?.Invoke(Turn = ++Turn % TurnCount);
    }
    
    public static void Clear() => Turn = 0;
}
