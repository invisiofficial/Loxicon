using System;

using UnityEngine;

public class ChatParamsWindow : MonoBehaviour
{
    #region Events
    
    public event Action<Preset<ChatParams>> OnClosed;
    
    #endregion

    private Preset<ChatParams> _currentPreset;
    
    private void Start() => this.GetComponentInChildren<ChatParamsPresetFactory>().OnPresetChosen += (Preset<ChatParams> preset) => _currentPreset = preset;
    
    public void Close() => OnClosed?.Invoke(_currentPreset);
}
