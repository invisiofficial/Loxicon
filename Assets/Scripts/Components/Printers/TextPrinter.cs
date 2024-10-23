using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class TextPrinter : MonoBehaviour
{
    public string Message;

    [Header("Events")]
    [Space]
    [SerializeField] private UnityEvent onValueChanged;

    [Space]
    [Header("Printer settings")]
    [SerializeField] private TMP_Text text;

    [SerializeField] private float interval = 0.03f;
    
    private int _currentPosition;
    
    private bool _isPrinting;

    private void Awake()
    {
        Message = text.text;
        text.text = string.Empty;
    }

    public void Print() => StartCoroutine(PrintMessage(string.Empty));
    public void Print(string messsage) => StartCoroutine(PrintMessage(messsage));

    private IEnumerator PrintMessage(string messsage)
    {
        // Applying message
        Message += messsage;
        
        // Checking for already printing
        if (_isPrinting) yield break;
        _isPrinting = true;
        
        // Print with interval
        while (_currentPosition < Message.Length - 1 && interval != 0.0f)
        {
            text.text = Message[..++_currentPosition];

            onValueChanged?.Invoke();

            yield return new WaitForSeconds(interval);
        }
        
        // Setting full message
        text.text = Message;
        _currentPosition = Message.Length;
        
        onValueChanged?.Invoke();
        
        _isPrinting = false;
    }
    
    public void Clear()
    {
        text.text = string.Empty;
        _currentPosition = 0;
    }
    
    public void Copy() => GUIUtility.systemCopyBuffer = Message;
}