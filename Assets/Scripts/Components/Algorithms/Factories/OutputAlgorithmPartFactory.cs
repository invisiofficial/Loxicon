using UnityEngine;

using TMPro;

using Invisi.Pseudocode;

public class OutputAlgorithmPartFactory : MonoBehaviour, IAlgorithmPartFactory, ISerializable
{
    [SerializeField] private TMP_InputField outputVariableNameInputField;
    
    public IAlgorithmPart Create() => new OutputAlgorithmPart(outputVariableNameInputField.text);
    
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
