using System;

using UnityEngine;

using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupAnimatee : TweenAnimatee
{
    [Serializable]
    private class CanvasGroupAnimateeBehavior
    {
        public float Alpha;
        public float Duration;
        public Ease EaseType;
    }

    [Header("Animatee settings")]
    [Space]
    [SerializeField] private CanvasGroupAnimateeBehavior[] behaviors;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public override Tween Animate(int index)
    {
        return _canvasGroup.SetProperties(behaviors[index].Alpha, behaviors[index].Duration, behaviors[index].EaseType);
    }
}
