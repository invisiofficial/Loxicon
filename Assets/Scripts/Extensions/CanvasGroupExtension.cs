using UnityEngine;

using DG.Tweening;

public static class CanvasGroupExtension
{    
    public static Tween SetProperties(this CanvasGroup group, float alpha, float duration, Ease easeType = Ease.Linear)
    {        
        // Tweening alpha
        Tween tween = group.DOFade(alpha, duration).SetEase(easeType);
        tween.onPlay = () =>
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        };
        tween.onComplete = () =>
        {
            if (alpha < 0.5f) return;
            
            group.interactable = true;
            group.blocksRaycasts = true;
        };
        
        // Returning tweener
        return tween;
    }
}
