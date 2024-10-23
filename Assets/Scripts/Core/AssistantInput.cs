using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class AssistantInput : MonoBehaviour
{
    #region Singleton implementation

    public static AssistantInput Instance;
    private void Awake() => Instance = this;

    #endregion

    #region Events

    public event Action<string> OnModelChosen;

    public event Action OnGenerationStarted;
    public event Action<string> OnGeneration;
    public event Action OnGenerationEnded;

    #endregion
    
    private const string DefaultModelNamePath = "Models";

    private const uint DefaultContextSize = 2048;
    private const int DefaultGpuLayerCount = 16;

    private const float DefaultTemperature = 0.6f;
    private const int DefaultMaxTokens = 1024;

    private const string DefaultContextPath = "Contexts";

    [Header("Configurator UI settings")]
    [Space]
    [SerializeField] private TMP_Dropdown modelNameDropdown;

    [SerializeField] private TMP_Dropdown executorDropdown;

    [SerializeField] private TMP_InputField contextSizeInputField;
    [SerializeField] private TMP_InputField gpuLayerCountInputField;

    [SerializeField] private TMP_InputField temperatureInputField;
    [SerializeField] private TMP_InputField maxTokensInputField;

    [SerializeField] private TMP_Dropdown contextDropdown;

    private Button _launchButton;

    private IDisposable _assistantChat;

    private void Start()
    {
        // Getting references
        _launchButton = this.GetComponent<Button>();
        
        // Updating model names
        UpdateModelNames();

        // Updating executors
        UpdateExecutors();

        // Setting default context size
        contextSizeInputField.placeholder.GetComponent<TMP_Text>().text = DefaultContextSize.ToString();
        // Setting default gpu layer count
        gpuLayerCountInputField.placeholder.GetComponent<TMP_Text>().text = DefaultGpuLayerCount.ToString();

        // Setting default temperature
        temperatureInputField.placeholder.GetComponent<TMP_Text>().text = DefaultTemperature.ToString();
        // Setting default max tokens
        maxTokensInputField.placeholder.GetComponent<TMP_Text>().text = DefaultMaxTokens.ToString();

        // Updating contexts
        UpdateContexts();
    }
    
    public void UpdateModelNames()
    {
        // Setting available models
        modelNameDropdown.ClearOptions();
        List<string> names = (from file in new DirectoryInfo(Application.streamingAssetsPath + "/" + DefaultModelNamePath).GetFiles() where file.Name.EndsWith(".gguf") select file.Name).ToList();
        modelNameDropdown.interactable = false;
        _launchButton.interactable = false;
        if (names.Count != 0)
        {
            modelNameDropdown.AddOptions(names);
            
            modelNameDropdown.interactable = true;
            _launchButton.interactable = true;
        }
    }
    
    public void UpdateExecutors()
    {
        // Setting available executors
        executorDropdown.ClearOptions();
        executorDropdown.AddOptions(Enum.GetNames(typeof(ExecutorType)).ToList());
    }
    
    public void UpdateContexts()
    {
        // Setting available contexts
        contextDropdown.ClearOptions();
        List<string> contexts = (from file in new DirectoryInfo(Application.streamingAssetsPath + "/" + DefaultContextPath).GetFiles() where file.Name.EndsWith(".json") select file.Name).ToList();
        contextDropdown.interactable = false;
        _launchButton.interactable = false;
        if (contexts.Count != 0)
        {
            contextDropdown.AddOptions(contexts);
            
            contextDropdown.interactable = true;
            _launchButton.interactable = true;
        }
    }

    private AssistantParams Get()
    {
        // Checking for input
        uint contextSize = DefaultContextSize;
        if (contextSizeInputField.text != string.Empty && (!uint.TryParse(contextSizeInputField.text, out contextSize))) throw new FormatException("Context size was incorrect!");
        int gpuLayerCount = DefaultGpuLayerCount;
        if (gpuLayerCountInputField.text != string.Empty && (!int.TryParse(gpuLayerCountInputField.text, out gpuLayerCount) || gpuLayerCount < 0)) throw new FormatException("GPU layers were incorrect!");

        float temperature = DefaultTemperature;
        if (temperatureInputField.text != string.Empty && (!float.TryParse(temperatureInputField.text, out temperature) || temperature < 0.0f || temperature > 1.0f)) throw new FormatException("Temperature was incorrect!");
        int maxTokens = DefaultMaxTokens;
        if (maxTokensInputField.text != string.Empty && (!int.TryParse(maxTokensInputField.text, out maxTokens) || maxTokens < 0)) throw new FormatException("Max tokens value was incorrect!");

        // Creating configuration
        return new(DefaultModelNamePath + "/" + modelNameDropdown.options[modelNameDropdown.value].text, (ExecutorType)executorDropdown.value, contextSize, gpuLayerCount, temperature, maxTokens, DefaultContextPath + "/" + contextDropdown.options[contextDropdown.value].text);
    }

    public async void Launch()
    {
        // Trying to launch assistant
        try
        {
            // Getting configuration
            AssistantParams assistantParams = Get();

            // Invoking event
            string[] names = modelNameDropdown.options[modelNameDropdown.value].text.Split('-');
            OnModelChosen?.Invoke(names[0] + ": " + names[2]);

            // Launching chat
            AssistantChat assistantChat = new(assistantParams);
            assistantChat.OnGenerationStarted += OnGenerationStarted;
            assistantChat.OnGeneration += OnGeneration;
            assistantChat.OnGenerationEnded += OnGenerationEnded;
            _assistantChat = assistantChat;
            await assistantChat.Inference();
        }
        catch (Exception)
        {
            // TODO: Change to exception handler of some type (example: window with exception message)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public static void Dispose() => Instance.OnDestroy();
    private void OnDestroy() => _assistantChat?.Dispose();
}