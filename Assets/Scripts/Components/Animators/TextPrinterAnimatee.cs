using System;

using UnityEngine;

using DG.Tweening;

[RequireComponent(typeof(TextPrinter))]
public class TextPrinterAnimatee : TweenAnimatee
{
    [Serializable]
    private class TextPrinterAnimateeBehavior
    {
        public string Text;
    }

    [Header("Animatee settings")]
    [Space]
    [SerializeField] private TextPrinterAnimateeBehavior[] behaviors;
    
    private TextPrinter _textPrinter;

    private void Awake()
    {
        _textPrinter = this.GetComponent<TextPrinter>();
    }
    
    public override Tween Animate(int index)
    {
        // Creating sequence
        Sequence sequence = DOTween.Sequence();
        
        // Printing text
        Action action = behaviors[index].Text != string.Empty ? () => _textPrinter.Clear() : () => _textPrinter.Print(behaviors[index].Text);
        sequence.onComplete = () => action?.Invoke();
        
        // Returning tween
        return sequence;
    }
}
