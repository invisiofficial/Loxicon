using UnityEngine;

using DG.Tweening;

public abstract class TweenAnimatee : MonoBehaviour
{
    public abstract Tween Animate(int index);
}
