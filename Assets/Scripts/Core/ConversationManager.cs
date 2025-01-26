using System;

using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    #region Singleton implementation

    public static ConversationManager Instance;
    private void Awake() => Instance = this;

    #endregion
    
    #region Events
    
    public event Func<int, Action<string>> OnMessage;
    
    #endregion
    
    public static Action<string> Message(int sender) => Instance.OnMessage?.Invoke(sender);
}
