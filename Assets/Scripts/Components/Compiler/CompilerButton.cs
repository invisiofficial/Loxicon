using UnityEngine;
using UnityEngine.UI;

public class CompilerButton : MonoBehaviour
{
    private Button _button;
    
    private void Awake() => _button = this.GetComponent<Button>();
    private void Start() => FindFirstObjectByType<Compiler>().OnCompilation += (bool result) => _button.enabled = result;
}
