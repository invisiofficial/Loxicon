using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class AssistantConfigurator : MonoBehaviour
{
    #region Events

    public static event Action<string> OnModelChosen;
    
    #endregion
    
    [Header("Configurator UI settings")]
    [Space]
    [SerializeField] private TMP_Dropdown modelNameDropdown;
    
    [SerializeField] private TMP_Dropdown executorDropdown;
    
    [SerializeField] private TMP_InputField contextSizeInputField;
    [SerializeField] private TMP_InputField gpuLayerCountInputField;
    
    [SerializeField] private TMP_InputField temperatureInputField;
    [SerializeField] private TMP_InputField maxTokensInputField;
    
    [SerializeField] private TMP_InputField systemContextInputField;
    
    private const uint DefaultContextSize = 2048;
    private const int DefaultGpuLayerCount = 16;
    
    private const float DefaultTemperature = 0.6f;
    private const int DefaultMaxTokens = 1024;
    
    private const string DefaultSystemContext = "You are an all-knowning and very responsive assistant who always answers accurately with the least words possible. Use only English language.";
    
    private void Start()
    {
        // Setting available models
        modelNameDropdown.ClearOptions();
        
        List<string> names = (from file in new DirectoryInfo(Application.streamingAssetsPath).GetFiles() where file.Name.EndsWith(".gguf") select file.Name).ToList();
        if (names.Count != 0)
        {
            modelNameDropdown.AddOptions(names);
        }
        else 
        {
            modelNameDropdown.interactable = false;
            this.GetComponent<Button>().interactable = false;
        }
        
        // Setting available executors
        executorDropdown.ClearOptions();
        executorDropdown.AddOptions(Enum.GetNames(typeof(ExecutorType)).ToList());
        
        // Setting default context size
        contextSizeInputField.placeholder.GetComponent<TMP_Text>().text = DefaultContextSize.ToString();
        // Setting default gpu layer count
        gpuLayerCountInputField.placeholder.GetComponent<TMP_Text>().text = DefaultGpuLayerCount.ToString();
        
        // Setting default temperature
        temperatureInputField.placeholder.GetComponent<TMP_Text>().text = DefaultTemperature.ToString();
        // Setting default max tokens
        maxTokensInputField.placeholder.GetComponent<TMP_Text>().text = DefaultMaxTokens.ToString();
        
        // Setting default system context
        systemContextInputField.placeholder.GetComponent<TMP_Text>().text = DefaultSystemContext;
    }
    
    private bool TryGet(out AssistantParams assistantParams)
    {
        assistantParams = null;
        
        // Checking for input
        uint contextSize = DefaultContextSize;
        if (contextSizeInputField.text != string.Empty && (!uint.TryParse(contextSizeInputField.text, out contextSize))) return false;
        int gpuLayerCount = DefaultGpuLayerCount;
        if (gpuLayerCountInputField.text != string.Empty && (!int.TryParse(gpuLayerCountInputField.text, out gpuLayerCount) || gpuLayerCount < 0)) return false;
    
        float temperature = DefaultTemperature;
        if (temperatureInputField.text != string.Empty && (!float.TryParse(temperatureInputField.text, out temperature) || temperature < 0.0f || temperature > 1.0f)) return false;
        int maxTokens = DefaultMaxTokens;
        if (maxTokensInputField.text != string.Empty && (!int.TryParse(maxTokensInputField.text, out maxTokens) || maxTokens < 0)) return false;
        
        string systemContext = systemContextInputField.text;
        if (systemContext == string.Empty) systemContext = DefaultSystemContext;
        
        // Creating configuration
        assistantParams = new(modelNameDropdown.options[modelNameDropdown.value].text, (ExecutorType)executorDropdown.value, contextSize, gpuLayerCount, temperature, maxTokens, systemContext);
        
        return true;
    }
    
    public async void Launch()
    {        
        // Getting configuration
        if (!TryGet(out AssistantParams assistantParams)) return;
        
        // Invoking event
        string[] names = assistantParams.ModelName.Split('-');
        OnModelChosen?.Invoke(names[0] + ": " + names[2]);
        
        // Trying to launch assistant
        try
        {
            await AssistantInput.Initialize(assistantParams);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}