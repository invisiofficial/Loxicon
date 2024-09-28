using UnityEngine;

using TMPro;

public class ConversationManager : MonoBehaviour
{
    #region Singleton implementation
    
    public static ConversationManager Instance;
    private void Awake() => Instance = this;
    
    #endregion
    
    [SerializeField] private TMP_InputField _inputField;
    
    [SerializeField] private Transform uiParent;
    
    private GameObject _informationUser;
    private GameObject _informationTalker;
    
    private void Start()
    {
        _informationUser = Resources.Load("Prefabs/InformationUser") as GameObject;
        _informationTalker = Resources.Load("Prefabs/InformationTalker") as GameObject;
    }
    
    private void OnMessage(string message, bool talker)
    {
        // Configuring input
        _inputField.enabled = talker;
        
        // Creating message object
        GameObject prefab;
        if (talker) prefab = _informationTalker;
        
        else prefab = _informationUser;
        
        GameObject gameObject = Instantiate(prefab, uiParent);
        gameObject.GetComponent<MessagePrinter>().Print(message);
        
        // Calling talker
        if (!talker) LLMManager.Inference(message);
    }

    public static void Message(string message, bool talker) => Instance.OnMessage(message, talker);
}
