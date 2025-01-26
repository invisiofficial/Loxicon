using System;
using System.Threading.Tasks;

using UnityEngine;

using TMPro;

using Invisi.Pseudocode;
using Invisi.Pseudocode.Compiler;

public class InputAlgorithmPartFactory : MonoBehaviour, IAlgorithmPartFactory, ICompilable, ISerializable
{
    #region Events
    
    public event Action OnCompilationRequired;
    
    #endregion
    
    [SerializeField] private TMP_InputField inputVariableNameInputField;
    
    private void Start()
    {
        inputVariableNameInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
    }

    public IAlgorithmPart Create() => new InputAlgorithmPart(inputVariableNameInputField.text);
    
    public async Task<CompilationResult> Compile(CompilerContext context)
    {
        // Getting variable name
        string input = inputVariableNameInputField.text;
        
        // Checking for empty name
        if (string.IsNullOrEmpty(input)) return CompilationResult.Error;
        
        // Checking for correct name
        if (!input.StartsWith('@')) return CompilationResult.Error;
        
        // Checking for name existence
        if (!context.Variables.Contains(input.TrimStart('@'))) return CompilationResult.Error;
        
        // Returing success
        return CompilationResult.Success;
    }
    
    #region ISerializable implementation
    
    [System.Serializable]
    private class Serialization
    {
        public string InputVariableName;
    }

    public string Serialize()
    {
        Serialization data = new()
        {
            InputVariableName = inputVariableNameInputField.text
        };

        return JsonUtility.ToJson(data);
    }
    
    public void Deserialize(string serializedData)
    {
        Serialization data = JsonUtility.FromJson<Serialization>(serializedData);
        
        inputVariableNameInputField.text = data.InputVariableName;
    }
    
    #endregion
}
