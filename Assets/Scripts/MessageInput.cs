using System.Collections;

using UnityEngine;

using TMPro;

public class MessageInput : MonoBehaviour
{
    private TMP_InputField _inputField;
    
    private void Start()
    {
        // Getting references
        _inputField = this.GetComponent<TMP_InputField>();
        
        // Adding submit event
        _inputField.onSubmit.AddListener((message) => Submit(message));
        _inputField.onSubmit.AddListener((_) => Clear());
        
    }
    
    private void Submit(string message) => ConversationManager.Message(message, false);
    private void Clear() => StartCoroutine(WaitClear());
    
    private IEnumerator WaitClear()
    {
        yield return new WaitForEndOfFrame();
        
        _inputField.text = string.Empty;
    }
}
