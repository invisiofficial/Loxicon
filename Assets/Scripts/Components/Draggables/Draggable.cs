using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDraggable, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Events

    public event System.Action OnDeleted;
    public event System.Action OnPositionUpdated;

    #endregion

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Transform _parent;
    private int _currentIndex;
    private GameObject _placeholder;

    private IDraggable _lowerDraggable, _upperDraggable;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Checking for RMB
        if (eventData.button != PointerEventData.InputButton.Right) return;
        
        // Destroying placeholder if exists
        if (_placeholder != null) Destroy(_placeholder);

        // Destroying object
        Destroy(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Getting current parent and index
        _parent = transform.parent;
        _currentIndex = transform.GetSiblingIndex();

        // Creating placeholder
        _placeholder = new GameObject("Placeholder");
        _placeholder.transform.SetParent(_parent);
        _placeholder.transform.localScale = this.transform.localScale;
        RectTransform rectTransform = _placeholder.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);

        _placeholder.transform.SetSiblingIndex(_currentIndex);

        // Setting parent to canvas
        transform.SetParent(_canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Moving object with the mouse
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        // Setting new target position
        int newSiblingIndex = _parent.childCount;
        for (int i = 0; i < _parent.childCount; i++)
        {
            // Checking for the placeholder child
            Transform child = _parent.GetChild(i);
            if (child == _placeholder.transform) continue;

            // Checking for the common child
            if (_rectTransform.position.y > child.position.y)
            {
                newSiblingIndex = i;

                if (_placeholder.transform.GetSiblingIndex() < newSiblingIndex) newSiblingIndex--;

                break;
            }
        }

        // Checking for the limits
        if (_lowerDraggable != null) newSiblingIndex = Mathf.Min(newSiblingIndex, _lowerDraggable.GetIndex() - 1);
        if (_upperDraggable != null) newSiblingIndex = Mathf.Max(newSiblingIndex, _upperDraggable.GetIndex() + 1);

        // Setting placeholder position
        _placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Setting new position
        transform.SetParent(_parent);
        transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex());

        // Destroying placeholder
        Destroy(_placeholder);
        
        // Invoking the event
        OnPositionUpdated?.Invoke();
    }
    
    private void OnDestroy() => OnDeleted?.Invoke();

    #region IDraggable implementation

    public void SetLimits(IDraggable lowerDraggable, IDraggable upperDraggable) => (_lowerDraggable, _upperDraggable) = (lowerDraggable, upperDraggable);

    public int GetIndex() => this.transform.GetSiblingIndex();

    #endregion
}
