using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System;

public class ConversationPrinter : MonoBehaviour
{
    #region Singleton implementation

    public static ConversationPrinter Instance;
    private void Awake() => Instance = this;

    #endregion
    
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
    
    private List<GameObject> _messagePrefabs = new();
    
    private GameObject _generatingPrefab;
    
    private int _currentScale = 8;
    
    private bool _isResizing;
    
    private void Start()
    {
        // Getting references
        _messagePrefabs.Add(Resources.Load("Prefabs/BlockUser") as GameObject);
        _messagePrefabs.Add(Resources.Load("Prefabs/BlockAssistant") as GameObject);
        
        _generatingPrefab = Resources.Load("Prefabs/Generating") as GameObject;
        
        // Listening to conversation
        ConversationManager.Instance.OnMessage += CreateMessage;
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
    
    private Action<string> CreateMessage(int sender)
    {
        // Choosing prefab
        GameObject prefab = _messagePrefabs[sender];
        
        // Creating message object
        GameObject gameObject = Instantiate(prefab, this.transform);
        gameObject.transform.localScale = _currentScale * scaleFactor * Vector3.one;
        _blocks.Enqueue(gameObject);
        
        // Checking for blocks limit reached
        if (_blocks.Count > maxBlocks) Destroy(_blocks.Dequeue());
        
        // Creating updater delegate
        TextPrinter currentPrinter = gameObject.GetComponent<TextPrinter>();
        return (string message) => currentPrinter.Print(message);
    }
    
    public void MentionGenerating(bool create)
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
