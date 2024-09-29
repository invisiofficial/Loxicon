using System;

using UnityEngine;

using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class RectTransformAnimatee : TweenAnimatee
{
    [Serializable]
    private class RectTransformAnimateeBehavior
    {
        public Vector2 SizeDelta;
        public float Duration;
        public Ease EaseType;
    }

    [Header("Animatee settings")]
    [Space]
    [SerializeField] private RectTransformAnimateeBehavior[] behaviors;
    
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
    }

    public override Tween Animate(int index)
    {
        // Getting size delta
        Vector2 sizeDelta = behaviors[index].SizeDelta;
        
        // Setting size delta depending on value
        sizeDelta.x = sizeDelta.x < 0.0f ? _rectTransform.sizeDelta.x : sizeDelta.x;
        sizeDelta.y = sizeDelta.y < 0.0f ? _rectTransform.sizeDelta.y : sizeDelta.y;
        
        // Returning tween
        return _rectTransform.DOSizeDelta(sizeDelta, behaviors[index].Duration).SetEase(behaviors[index].EaseType);
    }
}
