using UnityEngine;

public class CompilerMark : MonoBehaviour
{
    private GameObject _mark;
    
    public void Mark(GameObject markPrefab)
    {
        // Unmarking
        Unmark();
        
        // Checking for empty mark
        if (markPrefab == null) return;
               
        // Creating new instance of mark
        _mark = Instantiate(markPrefab, this.transform);
    }
    
    public void Unmark()
    {
        // Deleting previous mark
        if (_mark != null) Destroy(_mark);
    }
}
