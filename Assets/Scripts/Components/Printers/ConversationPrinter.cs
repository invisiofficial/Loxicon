using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class ConversationPrinter : MonoBehaviour
{
    [Header("Conversation settings")]
    [Space]
    [SerializeField] private int maxBlocks = 25;
    
    [Space]
    [Header("Scaler settings")]
    [Space]
    [SerializeField] private int maxScale = 10;
    [SerializeField] private int minScale = 6;
    
    [SerializeField] private float scaleFactor = 0.1f;
    [SerializeField] private float scaleDuration = 0.1f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;
    
    private readonly Queue<GameObject> _blocks = new();
    
    private GameObject _generatingObject;
    
    private GameObject _blockUserPrefab;
    private GameObject _blockAssistantPrefab;
    
    private GameObject _generatingPrefab;
    
    private TextPrinter _currentPrinter;
    
    private int _currentScale = 8;
    
    private bool _isResizing;
    
    private void Start()
    {
        // Getting references
        _blockUserPrefab = Resources.Load("Prefabs/BlockUser") as GameObject;
        _blockAssistantPrefab = Resources.Load("Prefabs/BlockAssistant") as GameObject;
        
        _generatingPrefab = Resources.Load("Prefabs/Generating") as GameObject;
        
        // Listening to the user
        UserInput.Instance.OnSubmit += (string message) => CreateMessage(message, true);
        
        // Listening to the assistant
        AssistantInput.Instance.OnGenerationStarted += () => MentionGenerating(true);
        AssistantInput.Instance.OnGeneration += UpdateMessage;
        AssistantInput.Instance.OnGenerationEnded += () => MentionGenerating(false);
        
        // Listening to conversation
        ConversationManager.Instance.OnMessageReceived += (string message) => 
        {
            if (ConversationManager.Turn == 0) return;
            
            _currentPrinter.Message = message;
            UpdateMessage(string.Empty);
            _currentPrinter = null;
        };
    }
    
    private void Update()
    {
        // Checking for the resize condition
        if (!Input.GetKey(KeyCode.LeftControl) || Input.mouseScrollDelta.y == 0 || _blocks.Count == 0 || _isResizing) return;
        _isResizing = true;
        
        // Resizing messages
        GameObject[] blocks = _blocks.ToArray();
        if (Input.mouseScrollDelta.y > 0 && _currentScale < maxScale) _currentScale++;
        if (Input.mouseScrollDelta.y < 0 && _currentScale > minScale) _currentScale--;
        foreach (GameObject block in blocks) block.transform.DOScale(_currentScale * scaleFactor * Vector3.one, scaleDuration).SetEase(easeType).onUpdate = () => LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        
        // End of resizing
        DOTween.Sequence().AppendInterval(scaleDuration).onComplete = () => _isResizing = false;
    }
    
    private void CreateMessage(string message, bool user)
    {
        // Choosing prefab
        GameObject prefab;
        if (user) prefab = _blockUserPrefab;
        
        else prefab = _blockAssistantPrefab;
        
        // Creating message object
        GameObject gameObject = Instantiate(prefab, this.transform);
        gameObject.transform.localScale = _currentScale * scaleFactor * Vector3.one;
        _currentPrinter = gameObject.GetComponent<TextPrinter>();
        _currentPrinter.Print(message);
        if (user) _currentPrinter = null;
        _blocks.Enqueue(gameObject);
        
        // Checking for blocks limit reached
        if (_blocks.Count > maxBlocks) Destroy(_blocks.Dequeue());
    }
    
    private void UpdateMessage(string message)
    {
        // Creating assistant message
        if (_currentPrinter == null) CreateMessage(string.Empty, false);
        
        // Printing message
        _currentPrinter.Print(message);
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
        // Destroying blocks
        while (_blocks.Count > 0) Destroy(_blocks.Dequeue());
        
        // Destroying generating
        Destroy(_generatingObject);
    }
}
