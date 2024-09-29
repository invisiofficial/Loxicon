using System;
using System.Collections;

using UnityEngine;

using TMPro;

public class MessagePrinter : MonoBehaviour
{
    public event Action OnValueChanged;
    
    private TMP_Text _messageText;
    
    private void Initialize() => _messageText = this.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>();
    
    public void Print(string messsage) => StartCoroutine(PrintMessage(messsage));
    
    private IEnumerator PrintMessage(string messsage)
    {
        // Initializing component
        if (_messageText == null) Initialize();
        
        // Printing text
        for (int i = 1; i <= messsage.Length; i++)
        {
            _messageText.text = messsage[..i];
            
            OnValueChanged?.Invoke();
            
            yield return new WaitForSeconds(0.03f);
        }
    }
}
