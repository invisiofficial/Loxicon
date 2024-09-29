using System;

using UnityEngine;

using DG.Tweening;

public class TweenAnimator : MonoBehaviour
{
    [Serializable]
    private class TweenAnimatorParallel
    {
        [Serializable]
        public class TweenAnimatorSequence
        {
            public int Index;
            public TweenAnimatee Animator;
            
            public float Interval;
        }
        
        public TweenAnimatorSequence[] Sequences;
    }

    [Header("Animator settings")]
    [Space]
    [SerializeField] private TweenAnimatorParallel[] parallels;

    public void Animate()
    {
        foreach (var parallel in parallels)
        {
            // Creating sequence
            Sequence tweenSequence = DOTween.Sequence();

            // Tweening each animator
            foreach (var sequence in parallel.Sequences)
            {
                tweenSequence.Append(sequence.Animator.Animate(sequence.Index));
                
                if (sequence.Interval > 0.0f) tweenSequence.AppendInterval(sequence.Interval);
            }
        }
    }
}
