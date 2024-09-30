using System.Collections.Generic;
using UnityEngine;

public class ConversationPrinter : MonoBehaviour
{
    [Header("Conversation settings")]
    [Space]
    [SerializeField] private int maxBlocks = 25;
    
    private Queue<GameObject> _blocks = new();
    
    private GameObject _informationUser;
    private GameObject _informationAssistant;
    
    private void Start()
    {
        // Getting references
        _informationUser = Resources.Load("Prefabs/InformationUser") as GameObject;
        _informationAssistant = Resources.Load("Prefabs/InformationAssistant") as GameObject;
        
        // Listening to the conversation
        ConversationHandler.OnMessageReceived += PrintMessage;
    }
    
    private void PrintMessage(string message)
    {
        // Choosing prefab
        GameObject prefab;
        if (ConversationHandler.Turn == 0) prefab = _informationUser;
        
        else prefab = _informationAssistant;
        
        // Creating message object
        GameObject gameObject = Instantiate(prefab, this.transform);
        gameObject.GetComponent<TextPrinter>().Print(message);
        _blocks.Enqueue(gameObject);
        
        // Checking for blocks limit reached
        if (_blocks.Count > maxBlocks) Destroy(_blocks.Dequeue());
    }
}
