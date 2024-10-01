using System;

public static class ConversationHandler
{   
    #region Events
    
    public static event Action<string> OnMessageReceived;
    public static event Action<int> OnTurnChanged;
    
    #endregion
    
    public const int TurnCount = 2;
    public static int Turn = 0;
    
    public static void Message(string message)
    {
        OnMessageReceived?.Invoke(message);
        OnTurnChanged?.Invoke(Turn = ++Turn % TurnCount);
    }
    
    public static void Clear()
    {
        OnTurnChanged?.Invoke(Turn = 0);
    }
}
