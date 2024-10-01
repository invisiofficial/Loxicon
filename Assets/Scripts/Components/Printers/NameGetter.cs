using UnityEngine;

[RequireComponent(typeof(TextPrinter))]
public class NameGetter : MonoBehaviour
{
    private void Start() => AssistantConfigurator.Instance.OnModelChosen += Set;
    
    private void Set(string name) => this.GetComponent<TextPrinter>().Message = name;
}
