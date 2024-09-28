using System;
using System.Collections;

using UnityEngine;

using TMPro;

using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SizeableText : MonoBehaviour
{
    [Header("Resizing settings")]
    [Space]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private float heightFactor = 3;
    
    [Space]
    [Header("Tweening settings")]
    [Space]
    [Range(0.0f, 3.0f)]
    [SerializeField] private float duration = 0.5f;
    
    
    private RectTransform _rectTransform;
    
    private float _rectHeight;
    
    private int _lineCount;
    
    private bool _isResizing;
    
    private void Start()
    {
        // Getting references
        _rectTransform = this.GetComponent<RectTransform>();

        // Saving offsets
        _rectHeight = _rectTransform.sizeDelta.y / heightFactor;
        
        // Adding resize event
        this.GetComponent<MessagePrinter>().OnValueChanged += Resize;
    }
    
    private void Resize()
    {
        // Checking for enabled
        if (!textComponent.enabled) return; 
        
        // Checking for resizing
        if (_isResizing)
        {
            StopAllCoroutines();
            
            StartCoroutine(WaitForResize(Resize));
            
            return;
        }
        
        // Calculating line count
        int lineCount = textComponent.GetTextInfo(textComponent.text).lineCount;
        
        // Resizing if line number is changed
        if (lineCount != _lineCount)
        {
            _isResizing = true;
            
            _lineCount = lineCount;
            
            _rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, _rectHeight * (_lineCount + heightFactor - 1)), duration).onComplete = () =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent as RectTransform);
                
                _isResizing = false;
            };
        }
    }
    
    IEnumerator WaitForResize(Action action)
    {
        yield return new WaitUntil(() => !_isResizing);
        
        action?.Invoke();
    }
}
