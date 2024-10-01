using System.Collections.Generic;
using UnityEngine;

public class ConversationPrinter : MonoBehaviour
{
    [Header("Conversation settings")]
    [Space]
    [SerializeField] private int maxBlocks = 25;
    
    private Queue<GameObject> _blocks = new();
    
    private GameObject _generatingObject;
    
    private GameObject _blockUserPrefab;
    private GameObject _blockAssistantPrefab;
    
    private GameObject _generatingPrefab;
    
    private void Start()
    {
        // Getting references
        _blockUserPrefab = Resources.Load("Prefabs/BlockUser") as GameObject;
        _blockAssistantPrefab = Resources.Load("Prefabs/BlockAssistant") as GameObject;
        
        _generatingPrefab = Resources.Load("Prefabs/Generating") as GameObject;
        
        // Unlistening to the conversation
        ConversationHandler.OnMessageReceived -= CreateMessage;
        
        // Listening to the conversation
        ConversationHandler.OnMessageReceived += CreateMessage;
        
        // Listening to the generation
        AssistantInput.Instance.OnGenerationStarted += () => MentionGenerating(true);
        AssistantInput.Instance.OnGenerationEnded += () => MentionGenerating(false);
    }
    
    private void CreateMessage(string message)
    {
        // Choosing prefab
        GameObject prefab;
        if (ConversationHandler.Turn == 0) prefab = _blockUserPrefab;
        
        else prefab = _blockAssistantPrefab;
        
        // Creating message object
        GameObject gameObject = Instantiate(prefab, this.transform);
        TextPrinter textPrinter = gameObject.GetComponent<TextPrinter>();
        textPrinter.Message = message;
        textPrinter.Print();
        _blocks.Enqueue(gameObject);
        
        // Checking for blocks limit reached
        if (_blocks.Count > maxBlocks) Destroy(_blocks.Dequeue());
    }
    
    private void MentionGenerating(bool create)
    {
        if (create)
        {
            _generatingObject = Instantiate(_generatingPrefab, this.transform);
            _generatingObject.GetComponent<TextPrinter>().Print();
        }
        else
        {
            Destroy(_generatingObject);
        }
    }
    
    public void Clear()
    {
        while (_blocks.Count > 0) Destroy(_blocks.Dequeue());
    }
}
