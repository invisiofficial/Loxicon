using System.Collections.Generic;
using UnityEngine;

public class ConversationPrinter : MonoBehaviour
{
    [Header("Conversation settings")]
    [Space]
    [SerializeField] private int maxBlocks = 25;
    
    private Queue<GameObject> _blocks = new();
    
    private GameObject _blockUser;
    private GameObject _blockAssistant;
    
    private void Start()
    {
        // Getting references
        _blockUser = Resources.Load("Prefabs/BlockUser") as GameObject;
        _blockAssistant = Resources.Load("Prefabs/BlockAssistant") as GameObject;
        
        // Unlistening to the conversation
        ConversationHandler.OnMessageReceived -= CreateMessage;
        
        // Listening to the conversation
        ConversationHandler.OnMessageReceived += CreateMessage;
    }
    
    private void CreateMessage(string message)
    {
        // Choosing prefab
        GameObject prefab;
        if (ConversationHandler.Turn == 0) prefab = _blockUser;
        
        else prefab = _blockAssistant;
        
        // Creating message object
        GameObject gameObject = Instantiate(prefab, this.transform);
        TextPrinter textPrinter = gameObject.GetComponent<TextPrinter>();
        textPrinter.Message = message;
        textPrinter.Print();
        _blocks.Enqueue(gameObject);
        
        // Checking for blocks limit reached
        if (_blocks.Count > maxBlocks) Destroy(_blocks.Dequeue());
    }
    
    public void Clear()
    {
        while (_blocks.Count > 0) Destroy(_blocks.Dequeue());
    }
}
