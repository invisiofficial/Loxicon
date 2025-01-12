using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ColorFlicker : MonoBehaviour
{
    [SerializeField] private Color flickColor;
    
    private readonly List<Image> _images = new();
    private readonly List<Color> _previousColors = new();
    
    private bool _isFlicked;
    
    private void Awake()
    {
        // Getting all image components
        _images.AddRange(this.GetComponentsInChildren<Image>());
        
        // Getting all image colors
        _previousColors.AddRange(_images.Select(x => x.color));
    }
    
    public void Flick()
    {
        // Perforing flicking
        if (!_isFlicked) foreach (var image in _images) image.color = flickColor;

        else for (int i = 0; i < _images.Count; i++) _images[i].color = _previousColors[i];
        
        // Updating state
        _isFlicked = !_isFlicked;
    }
}
