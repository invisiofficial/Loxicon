using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class TextPrinter : MonoBehaviour
{
    [Header("Events")]
    [Space]
    [SerializeField] private UnityEvent onValueChanged;
    
    [Space]
    [Header("Printer settings")]
    [SerializeField] private TMP_Text text;
    
    [SerializeField] private float interval = 0.03f;
    
    public void Print(string messsage) => StartCoroutine(PrintMessage(messsage));
    
    private IEnumerator PrintMessage(string messsage)
    {        
        for (int i = 1; i <= messsage.Length; i++)
        {
            text.text = messsage[..i];
            
            onValueChanged?.Invoke();
            
            yield return new WaitForSeconds(interval);
        }
    }
}
