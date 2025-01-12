using UnityEngine;

using TMPro;

using Invisi.Pseudocode;

public class InferModelAlgorithmPartFactory : MonoBehaviour, IAlgorithmPartFactory, ISerializable
{
    [SerializeField] private TMP_InputField inputVariableNameInputField;
    [SerializeField] private TMP_InputField modelVariableNameInputField;
    [SerializeField] private TMP_InputField outputVariableNameInputField;

    public IAlgorithmPart Create() => new InferModelAlgorithmPart(inputVariableNameInputField.text, modelVariableNameInputField.text, outputVariableNameInputField.text);
    
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
