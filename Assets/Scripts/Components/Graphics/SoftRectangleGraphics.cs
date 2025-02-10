using UnityEngine;
using UnityEngine.UI;

public class SoftRectangleGraphics : AChangable
{
    [Header("Settings")]
    [Space]
    [Range(10.0f, 100.0f)]
    [SerializeField] private float cornerRadius = 50;

    [SerializeField] private Color elementsColor = Color.white;
    
    [SerializeField] private Sprite cornerSprite = null;

    public override void Recreate()
    {
        // Deleting previous structure
        while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);

        // Creating new structure based on parameters
        CreateCenter();
        CreateSides();
        CreateCorners();
    }
    
    private void CreateCenter()
    {
        CreateElement(name: "Center", 
                      anchorMin: Vector2.zero, 
                      anchorMax: Vector2.one, 
                      pivot: new Vector2(0.5f, 0.5f),
                      rotation: Quaternion.identity,
                      color: elementsColor,
                      offsetMin: new Vector2(cornerRadius, cornerRadius), 
                      offsetMax: new Vector2(-cornerRadius, -cornerRadius));
    }
    
    private void CreateSides()
    {
        CreateElement(name: "Top Side", 
                      anchorMin: new Vector2(0.0f, 1.0f), 
                      anchorMax: new Vector2(1.0f, 1.0f), 
                      pivot: new Vector2(0.5f, 1.0f),
                      rotation: Quaternion.identity,
                      color: elementsColor,
                      offsetMin: new Vector2(cornerRadius, 0.0f), 
                      offsetMax: new Vector2(-cornerRadius, 0.0f), 
                      sizeDelta: new Vector2(0.0f, cornerRadius));
        
        CreateElement(name: "Bottom Side", 
                      anchorMin: new Vector2(0.0f, 0.0f), 
                      anchorMax: new Vector2(1.0f, 0.0f), 
                      pivot: new Vector2(0.5f, 0.0f),
                      rotation: Quaternion.identity,
                      color: elementsColor,
                      offsetMin: new Vector2(cornerRadius, 0.0f), 
                      offsetMax: new Vector2(-cornerRadius, 0.0f), 
                      sizeDelta: new Vector2(0.0f, cornerRadius));
        
        CreateElement(name: "Left Side", 
                      anchorMin: new Vector2(0.0f, 0.0f), 
                      anchorMax: new Vector2(0.0f, 1.0f), 
                      pivot: new Vector2(0.0f, 0.5f),
                      rotation: Quaternion.identity,
                      color: elementsColor,
                      offsetMin: new Vector2(0.0f, cornerRadius), 
                      offsetMax: new Vector2(0.0f, -cornerRadius), 
                      sizeDelta: new Vector2(cornerRadius, 0.0f));
        
        CreateElement(name: "Right Side", 
                      anchorMin: new Vector2(1.0f, 0.0f), 
                      anchorMax: new Vector2(1.0f, 1.0f), 
                      pivot: new Vector2(1.0f, 0.5f),
                      rotation: Quaternion.identity,
                      color: elementsColor,
                      offsetMin: new Vector2(0.0f, cornerRadius), 
                      offsetMax: new Vector2(0.0f, -cornerRadius), 
                      sizeDelta: new Vector2(cornerRadius, 0.0f));
    }

    private void CreateCorners()
    {
        CreateElement(name: "Top Left Corner", 
                      anchorMin: new Vector2(0.0f, 1.0f), 
                      anchorMax: new Vector2(0.0f, 1.0f), 
                      pivot: Vector2.one,
                      rotation: Quaternion.Euler(0.0f, 0.0f, 90.0f),
                      color: elementsColor,
                      sizeDelta: new Vector2(cornerRadius, cornerRadius),
                      sprite: cornerSprite);
        
        CreateElement(name: "Top Right Corner", 
                      anchorMin: new Vector2(1.0f, 1.0f), 
                      anchorMax: new Vector2(1.0f, 1.0f), 
                      pivot: Vector2.one,
                      rotation: Quaternion.Euler(0.0f, 0.0f, 0.0f),
                      color: elementsColor,
                      sizeDelta: new Vector2(cornerRadius, cornerRadius),
                      sprite: cornerSprite);
        
        CreateElement(name: "Bottom Left Corner", 
                      anchorMin: new Vector2(0.0f, 0.0f), 
                      anchorMax: new Vector2(0.0f, 0.0f), 
                      pivot: Vector2.one,
                      rotation: Quaternion.Euler(0.0f, 0.0f, 180.0f),
                      color: elementsColor,
                      sizeDelta: new Vector2(cornerRadius, cornerRadius),
                      sprite: cornerSprite);
        
        CreateElement(name: "Bottom Right Corner", 
                      anchorMin: new Vector2(1.0f, 0.0f), 
                      anchorMax: new Vector2(1.0f, 0.0f), 
                      pivot: Vector2.one,
                      rotation: Quaternion.Euler(0.0f, 0.0f, -90.0f),
                      color: elementsColor,
                      sizeDelta: new Vector2(cornerRadius, cornerRadius),
                      sprite: cornerSprite);
    }
    
    private GameObject CreateElement(string name,
                                     Vector2 anchorMin,
                                     Vector2 anchorMax,
                                     Vector2 pivot,
                                     Quaternion rotation,
                                     Color color,
                                     Vector2? offsetMin = null,
                                     Vector2? offsetMax = null,
                                     Vector2? sizeDelta = null,
                                     Sprite sprite = null)
    {
        // Creating object
        GameObject element = new(name);
        element.transform.SetParent(this.transform, false);

        // Configuring rect
        RectTransform rt = element.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = rotation;
        if (offsetMin.HasValue) rt.offsetMin = offsetMin.Value;
        if (offsetMax.HasValue) rt.offsetMax = offsetMax.Value;
        if (sizeDelta.HasValue) rt.sizeDelta = new Vector2(sizeDelta.Value.x == 0.0f ? rt.sizeDelta.x : sizeDelta.Value.x, sizeDelta.Value.y == 0.0f ? rt.sizeDelta.y : sizeDelta.Value.y);

        // Configuring image
        Image img = element.AddComponent<Image>();
        img.color = color;
        img.sprite = sprite;
        img.type = Image.Type.Simple;

        return element;
    }
}
