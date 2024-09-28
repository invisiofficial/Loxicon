using System.Collections;
using System;

using UnityEngine;

using TMPro;

using DG.Tweening;

public class SizeableInputField : MonoBehaviour
{
    [Header("Resizing settings")]
    [Space]
    [Range(2, 5)]
    [SerializeField] private int maxLineCount = 3;
    
    [Space]
    [Header("Tweening settings")]
    [Space]
    [Range(0.0f, 3.0f)]
    [SerializeField] private float duration = 0.5f;
    
    private TMP_InputField _inputField;
    
    private RectTransform _rectTransform;
    
    private float _rectHeight;
    
    private int _lineCount;
    
    private bool _isResizing;
    
    private void Start()
    {
        // Getting references
        _rectTransform = this.GetComponent<RectTransform>();
        
        _inputField = this.GetComponent<TMP_InputField>();

        // Saving offsets
        _rectHeight = _rectTransform.sizeDelta.y;
        
        // Adding resize event
        _inputField.onValueChanged.AddListener((_) => Resize());
    }
    
    private void Resize()
    {
        // Checking for enabled
        if (!_inputField.enabled) return; 
        
        // Checking for resizing
        if (_isResizing)
        {
            StopAllCoroutines();
            
            StartCoroutine(WaitForResize(Resize));
            
            return;
        }
        
        // Calculating line count
        int lineCount = _inputField.textComponent.GetTextInfo(string.Empty).lineCount;
        
        // Enabling scrollbar if maxLineSize is reached
        if (lineCount > maxLineCount)
        {
            _inputField.verticalScrollbar.gameObject.SetActive(true);
            _inputField.textComponent.overflowMode = TextOverflowModes.Overflow;
            
            return;
        }
        _inputField.verticalScrollbar.gameObject.SetActive(false);
        _inputField.textComponent.overflowMode = TextOverflowModes.Page;
    
        // Resizing if line number is changed
        if (lineCount != _lineCount)
        {
            _isResizing = true;
            
            _lineCount = lineCount;
            
            _rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, _rectHeight * _lineCount), duration).onComplete = () => _isResizing = false;
        }
    }
    
    IEnumerator WaitForResize(Action action)
    {
        yield return new WaitUntil(() => !_isResizing);
        
        action?.Invoke();
    }
}
