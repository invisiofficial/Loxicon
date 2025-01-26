using System;
using System.Threading.Tasks;

using UnityEngine;

using TMPro;

using Invisi.Pseudocode;
using Invisi.Pseudocode.Compiler;

public class JumpAlgorithmPartFactory : MonoBehaviour, IAlgorithmPartFactory, ICompilable, ISerializable
{
    #region Events
    
    public event Action OnCompilationRequired;
    
    #endregion
    
    [SerializeField] private TMP_InputField conditionVariableNameInputField;
    [SerializeField] private TMP_InputField jumpIndexInputField;
    
    private void Start()
    {
        conditionVariableNameInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
        jumpIndexInputField.onValueChanged.AddListener(delegate { OnCompilationRequired?.Invoke(); });
    }
    
    public IAlgorithmPart Create() => new JumpAlgorithmPart(conditionVariableNameInputField.text, int.Parse(jumpIndexInputField.text));
    
    public async Task<CompilationResult> Compile(CompilerContext context)
    {
        // Getting variable name
        string name = conditionVariableNameInputField.text;
        
        // Checking for empty name
        if (string.IsNullOrEmpty(name)) return CompilationResult.Error;
        
        // Checking for correct names
        if (!name.StartsWith('@')) return CompilationResult.Error;
        
        // Checking for name existence
        if (!context.Variables.Contains(name.TrimStart('@')) && !name.Equals("@true") && !name.Equals("@false")) return CompilationResult.Error;
        
        // Getting jump index
        string jump = jumpIndexInputField.text;
        
        // Checking for empty index
        if (string.IsNullOrEmpty(jump)) return CompilationResult.Error;
        
        // Checking for correct type
        if (!int.TryParse(jump, out int jumpNumber)) return CompilationResult.Error;
        
        // Checking for correct jump index
        if (jumpNumber <= 0 || jumpNumber == context.CurrentIndex || jumpNumber > context.LastIndex) return CompilationResult.Error;
        
        // Returing success
        return CompilationResult.Success;
    }
    
    #region ISerializable implementation
    
    [System.Serializable]
    private class Serialization
    {
        public string ConditionVariableName;
        public string JumpIndex;
    }

    public string Serialize()
    {
        Serialization data = new()
        {
            ConditionVariableName = conditionVariableNameInputField.text,
            JumpIndex = jumpIndexInputField.text
        };

        return JsonUtility.ToJson(data);
    }
    
    public void Deserialize(string serializedData)
    {
        Serialization data = JsonUtility.FromJson<Serialization>(serializedData);
        
        conditionVariableNameInputField.text = data.ConditionVariableName;
        jumpIndexInputField.text = data.JumpIndex;
    }
    
    #endregion
}
