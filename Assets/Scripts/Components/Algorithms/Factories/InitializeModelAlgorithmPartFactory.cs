using UnityEngine;

using TMPro;

using Invisi.Pseudocode;

public class InitializeModelAlgorithmPartFactory : InitializeAlgorithmPartFactory<string>, IAlgorithmPartFactory, ISerializable
{
    [SerializeField] private GameObject chatParamsWindowPrefab;
    
    [Space]
    [SerializeField] private TMP_InputField modelVariableNameInputField;
    [SerializeField] private TMP_InputField chatParamsPresetInputField;
    
    private Preset<ChatParams> _chatParamsPreset;
        
    public void OpenChatParams()
    {
        GameObject gameObject = Instantiate(chatParamsWindowPrefab, FindObjectOfType<Canvas>().transform);
        ChatParamsWindow chatParamsWindow = gameObject.GetComponent<ChatParamsWindow>();
        chatParamsWindow.OnClosed += (Preset<ChatParams> preset) =>
        { 
            if (preset != null)
            {
                _chatParamsPreset = preset;
                chatParamsPresetInputField.text = preset.Name;
            }
            else
            {
                _chatParamsPreset = null;
                chatParamsPresetInputField.text = string.Empty;
            }
        };
        chatParamsWindow.OnClosed += (_) => Destroy(gameObject);
        gameObject.SetActive(true);
    }
    
    public IAlgorithmPart Create()
    {
        // Getting variable name
        string name = modelVariableNameInputField.text;
        
        // Invoking events
        OnVariableInitialized?.Invoke(name);
        
        // Returning the instance
        return new InitializeModelAlgorithmPart(name, _chatParamsPreset.Value);
    }
    
    #region ISerializable implementation
    
    [System.Serializable]
    private class Serialization
    {
        public string ModelVariableName;
        
        public Preset<ChatParams> ChatParamsPreset;
    }

    public string Serialize()
    {
        Serialization data = new()
        {
            ModelVariableName = modelVariableNameInputField.text,
            ChatParamsPreset = _chatParamsPreset
        };

        return JsonUtility.ToJson(data);
    }
    
    public void Deserialize(string serializedData)
    {
        Serialization data = JsonUtility.FromJson<Serialization>(serializedData);
        
        modelVariableNameInputField.text = data.ModelVariableName;
        _chatParamsPreset = data.ChatParamsPreset;
        
        if (_chatParamsPreset != null) chatParamsPresetInputField.text = _chatParamsPreset.Name;
    }
    
    #endregion
}
