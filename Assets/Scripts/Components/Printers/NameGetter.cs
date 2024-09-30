using UnityEngine;

[RequireComponent(typeof(TextPrinter))]
public class NameGetter : MonoBehaviour
{
    private void Start() => AssistantConfigurator.OnModelChosen += Set;
    
    private void Set(string name) => this.GetComponent<TextPrinter>().Message = name;
}
