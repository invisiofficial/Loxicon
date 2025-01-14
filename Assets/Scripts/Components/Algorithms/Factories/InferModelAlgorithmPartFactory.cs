using System;
using System.Threading.Tasks;

using UnityEngine;

using TMPro;

using Invisi.Pseudocode;
using Invisi.Pseudocode.Compiler;

public class InferModelAlgorithmPartFactory : MonoBehaviour, IAlgorithmPartFactory, ICompilable, ISerializable
{
    #region Events
    
    public event Action OnCompilationRequired;
    
    #endregion
    
    [SerializeField] private TMP_InputField inputVariableNameInputField;
    [SerializeField] private TMP_InputField modelVariableNameInputField;
    [SerializeField] private TMP_InputField outputVariableNameInputField;
    
    private void Start()
    {
        inputVariableNameInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
        modelVariableNameInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
        outputVariableNameInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
    }

    public IAlgorithmPart Create() => new InferModelAlgorithmPart(inputVariableNameInputField.text, modelVariableNameInputField.text, outputVariableNameInputField.text);
    
    public async Task<CompilationResult> Compile(CompilerContext context)
    {
        // Getting variable names
        string input = inputVariableNameInputField.text;
        string model = modelVariableNameInputField.text;
        string output = outputVariableNameInputField.text;
        
        // Checking for empty names
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(output)) return CompilationResult.Error;
        
        // Checking for correct names
        if (!input.StartsWith('@') || !model.StartsWith('@') || !output.StartsWith('@')) return CompilationResult.Error;
        
        // Checking for names existence
        if (!context.Variables.Contains(input.TrimStart('@')) || !context.Variables.Contains(model.TrimStart('@')) || !context.Variables.Contains(output.TrimStart('@'))) return CompilationResult.Error;
                
        // Returing success
        return CompilationResult.Success;
    }
    
    #region ISerializable implementation
    
    [System.Serializable]
    private class Serialization
    {
        public string InputVariableName;
        public string ModelVariableName;
        public string OutputVariableName;
    }

    public string Serialize()
    {
        Serialization data = new()
        {
            InputVariableName = inputVariableNameInputField.text,
            ModelVariableName = modelVariableNameInputField.text,
            OutputVariableName = outputVariableNameInputField.text
        };

        return JsonUtility.ToJson(data);
    }
    
    public void Deserialize(string serializedData)
    {
        Serialization data = JsonUtility.FromJson<Serialization>(serializedData);
        
        inputVariableNameInputField.text = data.InputVariableName;
        modelVariableNameInputField.text = data.ModelVariableName;
        outputVariableNameInputField.text = data.OutputVariableName;
    }
    
    #endregion
}
