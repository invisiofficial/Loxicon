using System;
using System.IO;
using System.Threading.Tasks;

using UnityEngine;

using TMPro;

using Invisi.Pseudocode;
using Invisi.Pseudocode.Compiler;

public class InitializeModelAlgorithmPartFactory : InitializeAlgorithmPartFactory<string>, IAlgorithmPartFactory, ICompilable, ISerializable
{
    #region Events
    
    public event Action OnCompilationRequired;
    
    #endregion
    
    [SerializeField] private GameObject chatParamsWindowPrefab;
    
    [Space]
    [SerializeField] private TMP_InputField modelVariableNameInputField;
    [SerializeField] private TMP_InputField chatParamsPresetInputField;
    
    private Preset<ChatParams> _chatParamsPreset;
    
    private void Start()
    {
        modelVariableNameInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
        chatParamsPresetInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
    }
        
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
        
        // Invoking event
        OnVariableInitialized?.Invoke(name);
        
        // Returning the instance
        return new InitializeModelAlgorithmPart(name, _chatParamsPreset.Value);
    }
    
    public async Task<CompilationResult> Compile(CompilerContext context)
    {
        // Getting variable name
        string name = modelVariableNameInputField.text;
        
        // Cheking for empty name
        if (string.IsNullOrEmpty(name)) return CompilationResult.Error;
        
        // Checking for correct name
        if (name.StartsWith('@')) return CompilationResult.Error;
        
        // Checking for variable with the same name
        if (context.Variables.Contains(name)) return CompilationResult.Error;
        
        // Checking for preset
        if (_chatParamsPreset == null) return CompilationResult.Error;
        
        // Getting chat params
        ChatParams chatParams = _chatParamsPreset.Value;
        
        // Checking for model file existence
        if (!File.Exists(Application.streamingAssetsPath + "/" + chatParams.ModelName)) return CompilationResult.Error;
        
        // Checking for context file existence
        if (!File.Exists(Application.streamingAssetsPath + "/" + chatParams.Context)) return CompilationResult.Error;
        
        // Simulating work
        context.Variables.Add(name);
        
        // Invoking event
        OnVariableInitialized?.Invoke(name);
        
        // Returing success
        return CompilationResult.Success;
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
