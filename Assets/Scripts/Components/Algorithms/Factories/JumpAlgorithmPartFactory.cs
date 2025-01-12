using UnityEngine;

using TMPro;

using Invisi.Pseudocode;

public class JumpAlgorithmPartFactory : MonoBehaviour, IAlgorithmPartFactory, ISerializable
{
    [SerializeField] private TMP_InputField conditionVariableNameInputField;
    [SerializeField] private TMP_InputField jumpIndexInputField;
    
    public IAlgorithmPart Create() => new JumpAlgorithmPart(conditionVariableNameInputField.text, int.Parse(jumpIndexInputField.text));
    
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
