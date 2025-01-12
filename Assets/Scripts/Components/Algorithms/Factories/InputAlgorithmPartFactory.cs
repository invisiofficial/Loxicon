using UnityEngine;

using TMPro;

using Invisi.Pseudocode;

public class InputAlgorithmPartFactory : MonoBehaviour, IAlgorithmPartFactory, ISerializable
{
    [SerializeField] private TMP_InputField inputVariableNameInputField;

    public IAlgorithmPart Create() => new InputAlgorithmPart(inputVariableNameInputField.text);
    
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
