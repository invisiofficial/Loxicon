using System;

public class AlgorithmVariable
{
    #region Events
    
    public event Action OnValueChanged;
    
    #endregion
    
    public object Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            
            OnValueChanged?.Invoke();
        }
    }
    public bool Free { get => _free; set => _free = value; }
    
    private object _value;
    private bool _free;
    
    public AlgorithmVariable(object value, bool free = true)
    {
        Value = value;
        Free = free;
    }
}
