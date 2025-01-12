using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class PresetFactoryWorker : MonoBehaviour, IPointerClickHandler
{
    #region Events
    
    public event Action OnLeftClick;
    public event Action OnRightClick;
    
    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) OnLeftClick?.Invoke();
        if (eventData.button == PointerEventData.InputButton.Right) OnRightClick?.Invoke();
    }
}
