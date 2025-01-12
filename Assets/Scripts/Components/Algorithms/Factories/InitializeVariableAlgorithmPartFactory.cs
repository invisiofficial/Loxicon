using UnityEngine;

using TMPro;

using Invisi.Pseudocode;

public class InitializeVariableAlgorithmPartFactory : InitializeAlgorithmPartFactory<string>, IAlgorithmPartFactory, ISerializable
{
    [SerializeField] private TMP_InputField variableNameInputField;
    
    public IAlgorithmPart Create()
    {
        // Getting variable name
        string name = variableNameInputField.text;
        
        // Invoking events
        OnVariableInitialized?.Invoke(name);
        
        // Returning the instance
        return new InitializeVariableAlgorithmPart(name);
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
