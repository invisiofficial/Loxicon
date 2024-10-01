using System.Collections;

using UnityEngine;

using TMPro;

public class UserInput : MonoBehaviour
{
    #region Singleton implementation
    
    public static UserInput Instance;
    private void Awake() => Instance = this;
    
    #endregion
    
    // These questions were generated by AI :)
    private readonly string[] defaultMessages = new string[]
    {
        "What is the highest mountain in the world?",
        "Which planet in our solar system has the most moons?",
        "Who painted the Mona Lisa?",
        "What is the capital of Peru?",
        "How many sides does a heptagon have?",
        "What is the boiling point of water at sea level?",
        "In which year did the first moon landing occur?",
        "Which element in the periodic table has the symbol Au?",
        "Who wrote 'Romeo and Juliet'?",
        "What is the tallest building in the world?",
        "How many legs does a spider have?",
        "What is the largest country by area?",
        "In which year did World War II end?",
        "Which gas do plants absorb from the atmosphere during photosynthesis?",
        "Who discovered penicillin?",
        "What is the capital of Australia?",
        "How many continents are there in the world?",
        "What is the chemical formula for water?",
        "In which year did the first smartphone (Nokia 3310) come out?",
        "Which planet has rings made mostly of ice and rock?",
        "Who invented the light bulb?",
        "How many colors are in the standard rainbow?",
        "What is the strongest natural material known to humans?",
        "In which year did the first Olympic Games take place?",
        "Which animal has the longest lifespan?"
    };

    private TMP_InputField _inputField;
    
    private bool _isAvailable = true;

    private void Start()
    {
        // Getting references
        _inputField = this.GetComponent<TMP_InputField>();

        // Listening to the conversation
        ConversationHandler.OnTurnChanged += SetAvailable;
        
        // Adding submit event
        _inputField.onSubmit.AddListener((message) => SubmitMessage(message));
    }
    
    private void SetAvailable(int turn) => _isAvailable = turn == 0;

    private void SubmitMessage(string message)
    {
        // Cheking for available
        if (!_isAvailable) return;
        
        // Sending message
        ConversationHandler.Message(message != string.Empty ? message : defaultMessages[Random.Range(0, defaultMessages.Length)]);
        
        // Clearing input field
        StartCoroutine(WaitClear());
        IEnumerator WaitClear()
        {
            _inputField.textComponent.enabled = false;
            
            yield return new WaitForEndOfFrame();
            
            _inputField.text = string.Empty;
            
            _inputField.textComponent.enabled = true;
        }   
    }
    
    public static void ResetState()
    {
        Instance._isAvailable = true;
        ConversationHandler.Turn = 0;
    }
}