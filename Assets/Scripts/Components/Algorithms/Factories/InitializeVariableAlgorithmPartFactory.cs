using System;
using System.Threading.Tasks;

using UnityEngine;

using TMPro;

using Invisi.Pseudocode;
using Invisi.Pseudocode.Compiler;

public class InitializeVariableAlgorithmPartFactory : InitializeAlgorithmPartFactory<string>, IAlgorithmPartFactory, ICompilable, ISerializable
{
    #region Events
    
    public event Action OnCompilationRequired;
    
    #endregion
    
    [SerializeField] private TMP_InputField variableNameInputField;
    
    private void Start()
    {
        variableNameInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
    }
    
    public IAlgorithmPart Create()
    {
        // Getting variable name
        string name = variableNameInputField.text;
        
        // Invoking event
        OnVariableInitialized?.Invoke(name);
        
        // Returning the instance
        return new InitializeVariableAlgorithmPart(name);
    }
    
    public async Task<CompilationResult> Compile(CompilerContext context)
    {
        // Getting variable name
        string name = variableNameInputField.text;
        
        // Cheking for empty name
        if (string.IsNullOrEmpty(name)) return CompilationResult.Error;
        
        // Checking for correct name
        if (name.StartsWith('@')) return CompilationResult.Error;
        
        // Checking for variable with the same name
        if (context.Variables.Contains(name)) return CompilationResult.Error;
        
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
        public string VariableName;
    }

    public string Serialize()
    {
        Serialization data = new()
        {
            VariableName = variableNameInputField.text
        };

        return JsonUtility.ToJson(data);
    }
    
    public void Deserialize(string serializedData)
    {
        Serialization data = JsonUtility.FromJson<Serialization>(serializedData);
        
        variableNameInputField.text = data.VariableName;
    }
    
    #endregion
}
