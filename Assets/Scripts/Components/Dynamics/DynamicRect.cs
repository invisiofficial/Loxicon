using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

using DG.Tweening;

public class DynamicRect : MonoBehaviour
{
    [Header("Events")]
    [Space]
    [SerializeField] private UnityEvent<bool> onResize;

    [Space]
    [Header("Resizing settings")]
    [Space]
    [SerializeField] private TMP_Text textComponent;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float heightFactor = 1;

    [SerializeField] private int maxLinesCount = 0;

    [Space]
    [Header("Tweening settings")]
    [Space]
    [Range(0.0f, 3.0f)]
    [SerializeField] private float duration = 0.5f;

    private float _rectHeight;

    private int _linesCount = 1;

    private bool _isResizing;

    private void Awake()
    {
        // Saving offsets
        _rectHeight = rectTransform.sizeDelta.y / heightFactor;
    }

    public void Resize()
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
        bool maxLinesReached = lineCount > maxLinesCount && maxLinesCount > 0;

        // Invoking event
        onResize?.Invoke(maxLinesReached);

        // Resizing if line number is changed
        if (lineCount == _linesCount) return;
        if (maxLinesReached)
        {
            if (_linesCount < maxLinesCount) lineCount = maxLinesCount;
            
            else return;
        }
        _linesCount = lineCount;

        _isResizing = true;

        Tween tween = rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, _rectHeight * (_linesCount + heightFactor - 1)), duration);
        tween.onUpdate = () => LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform.parent as RectTransform);
        tween.onComplete = () => _isResizing = false;
    }

    IEnumerator WaitForResize(Action action)
    {
        yield return new WaitUntil(() => !_isResizing);

        action?.Invoke();
    }
}
