using System;

using UnityEngine;

using TMPro;

using DG.Tweening;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldAnimatee : TweenAnimatee
{
    [Serializable]
    private class InputFieldAnimateeBehavior
    {
        public bool Interactable;
    }

    [Header("Animatee settings")]
    [Space]
    [SerializeField] private InputFieldAnimateeBehavior[] behaviors;
    
    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = this.GetComponent<TMP_InputField>();
    }
    
    public override Tween Animate(int index)
    {
        // Creating sequence
        Sequence sequence = DOTween.Sequence();
        
        // Switching enable
        sequence.onComplete = () => _inputField.interactable = behaviors[index].Interactable;
        
        // Returning tween
        return sequence;
    }
}
