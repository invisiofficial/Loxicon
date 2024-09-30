using UnityEngine;

using TMPro;

[RequireComponent(typeof(TMP_InputField), typeof(DynamicRect))]
public class DynamicInputField : MonoBehaviour
{
    private TMP_InputField _inputField;
    
    private void Start() => _inputField = this.GetComponent<TMP_InputField>();
    
    public void Resize(bool maxLinesReached)
    {
        if (maxLinesReached)
        {
            _inputField.verticalScrollbar.gameObject.SetActive(true);
            _inputField.textComponent.overflowMode = TextOverflowModes.Overflow;
        }
        else
        {
            _inputField.verticalScrollbar.gameObject.SetActive(false);
            _inputField.textComponent.overflowMode = TextOverflowModes.Page;
        }
    }
}
