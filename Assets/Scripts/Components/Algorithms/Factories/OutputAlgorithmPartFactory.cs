using System;
using System.Threading.Tasks;

using UnityEngine;

using TMPro;

using Invisi.Pseudocode;
using Invisi.Pseudocode.Compiler;

public class OutputAlgorithmPartFactory : MonoBehaviour, IAlgorithmPartFactory, ICompilable, ISerializable
{
    #region Events
    
    public event Action OnCompilationRequired;
    
    #endregion
    
    [SerializeField] private TMP_InputField outputVariableNameInputField;
    
    private void Start()
    {
        outputVariableNameInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
    }
    
    public IAlgorithmPart Create() => new OutputAlgorithmPart(outputVariableNameInputField.text);
    
    public async Task<CompilationResult> Compile(CompilerContext context)
    {
        // Getting variable name
        string output = outputVariableNameInputField.text;
        
        // Checking for empty name
        if (string.IsNullOrEmpty(output)) return CompilationResult.Error;
        
        // Checking for correct name
        if (!output.StartsWith('@')) return CompilationResult.Error;
        
        // Checking for name existence
        if (!context.Variables.Contains(output.TrimStart('@'))) return CompilationResult.Error;
        
        // Returing success
        return CompilationResult.Success;
    }
    
    #region ISerializable implementation
    
    [System.Serializable]
    private class Serialization
    {
        public string OutputVariableName;
    }

    public string Serialize()
    {
        Serialization data = new()
        {
            OutputVariableName = outputVariableNameInputField.text
        };

        return JsonUtility.ToJson(data);
    }
    
    public void Deserialize(string serializedData)
    {
        Serialization data = JsonUtility.FromJson<Serialization>(serializedData);
        
        outputVariableNameInputField.text = data.OutputVariableName;
    }
    
    #endregion
}
