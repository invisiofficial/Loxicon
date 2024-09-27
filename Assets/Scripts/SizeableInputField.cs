using System.Collections;
using System;

using UnityEngine;
using UnityEngine.UI;

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
    
    private RectTransform _rectTransform;
    
    private TMP_InputField _inputField;
    private RectTransform _carretTransform;

    private float _rectHeight;
    
    private int _lineCount;
    
    private bool _isResizing;
    
    private void Start()
    {
        // Getting references
        _rectTransform = this.GetComponent<RectTransform>();
        _inputField = this.GetComponent<TMP_InputField>();
        _carretTransform = this.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        // Saving offsets
        _rectHeight = _rectTransform.sizeDelta.y;
        
        // Adding resize event
        _inputField.onValueChanged.AddListener((_) => Resize());
    }
    
    private void LateUpdate()
    {
        if (_isResizing)
        {
            _inputField.textComponent.rectTransform.anchoredPosition = Vector2.zero;
            _carretTransform.anchoredPosition = Vector2.zero;
        }
    }
    
    private void Resize()
    {
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
            
            return;
        }
        _inputField.verticalScrollbar.gameObject.SetActive(false);
        
        // Changing text pivot
        if (lineCount > _lineCount) 
        {
            _carretTransform.pivot = new(_carretTransform.pivot.x, 1.0f);
            _inputField.textComponent.rectTransform.pivot = new(_inputField.textComponent.rectTransform.pivot.x, 1.0f);
        }
        if (lineCount == _lineCount) 
        {
            _carretTransform.pivot = new(_carretTransform.pivot.x, 0.5f);
            _inputField.textComponent.rectTransform.pivot = new(_inputField.textComponent.rectTransform.pivot.x, 0.5f);
        }
        if (lineCount < _lineCount) 
        {
            _carretTransform.pivot = new(_carretTransform.pivot.x, 0.0f);
            _inputField.textComponent.rectTransform.pivot = new(_inputField.textComponent.rectTransform.pivot.x, 0.0f);
        }

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
