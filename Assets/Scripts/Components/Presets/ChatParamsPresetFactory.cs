using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class ChatParamsPresetFactory : PresetFactory<ChatParams>
{
    [Space]
    [Header("Configuration settings")]
    [SerializeField] private TMP_Dropdown modelNameDropdown;
    [SerializeField] private TMP_Dropdown executorDropdown;

    [SerializeField] private TMP_InputField contextSizeInputField;
    [SerializeField] private TMP_InputField gpuLayerCountInputField;

    [SerializeField] private TMP_InputField temperatureInputField;
    [SerializeField] private TMP_InputField maxTokensInputField;

    [SerializeField] private TMP_Dropdown contextDropdown;

    private const string DefaultPresetName = "Params";

    private const uint DefaultContextSize = 2048;
    private const int DefaultGpuLayerCount = 16;

    private const float DefaultTemperature = 0.6f;
    private const int DefaultMaxTokens = 1024;

    private const string DefaultModelNamePath = "Models";
    private const string DefaultContextPath = "Contexts";

    protected override void UpdateFields()
    {
        // Updating preset name
        presetNameInputField.placeholder.GetComponent<TMP_Text>().text = DefaultPresetName;
        
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

        void UpdateModelNames()
        {
            modelNameDropdown.ClearOptions();
            List<string> names = (from file in new DirectoryInfo(Application.streamingAssetsPath + "/" + DefaultModelNamePath).GetFiles()
                                  where file.Name.EndsWith(".gguf")
                                  select file.Name).ToList();
            modelNameDropdown.AddOptions(names);
        }

        void UpdateExecutors()
        {
            executorDropdown.ClearOptions();
            executorDropdown.AddOptions(Enum.GetNames(typeof(ExecutorType)).ToList());
        }

        void UpdateContexts()
        {
            contextDropdown.ClearOptions();
            List<string> contexts = (from file in new DirectoryInfo(Application.streamingAssetsPath + "/" + DefaultContextPath).GetFiles()
                                     where file.Name.EndsWith(".json")
                                     select file.Name).ToList();
            contextDropdown.AddOptions(contexts);
        }
    }

    protected override void UpdatePresets()
    {
        // Clearing buttons
        foreach (Transform child in this.transform) Destroy(child.gameObject);

        // Creating create button
        CreateCreateButton();

        // Creating preset buttons
        IReadOnlyList<Preset<ChatParams>> presets = _presetManager.GetPresets();
        for (int i = 0; i < presets.Count; i++) CreatePresetButton(i, presets[i].Name);
    }
    
    protected override void SetPreset(Preset<ChatParams> preset)
    {
        presetNameInputField.text = preset.Name;
        
        modelNameDropdown.value = modelNameDropdown.options.FindIndex(o => o.text == preset.Value.ModelName[(DefaultModelNamePath.Length + 1)..]);
        executorDropdown.value = (int)preset.Value.ExecutorType;

        contextSizeInputField.text = preset.Value.ContextSize.ToString();
        gpuLayerCountInputField.text = preset.Value.GpuLayerCount.ToString();

        temperatureInputField.text = preset.Value.Temperature.ToString();
        maxTokensInputField.text = preset.Value.MaxTokens.ToString();

        contextDropdown.value = contextDropdown.options.FindIndex(o => o.text == preset.Value.Context[(DefaultContextPath.Length + 1)..]);
    }

    protected override Preset<ChatParams> GetPreset()
    {
        string presetName = string.IsNullOrEmpty(presetNameInputField.text) ? DefaultPresetName : presetNameInputField.text;
        
        string modelName = DefaultModelNamePath + "/" + modelNameDropdown.options[modelNameDropdown.value].text;

        uint contextSize = string.IsNullOrEmpty(contextSizeInputField.text) ? DefaultContextSize : uint.Parse(contextSizeInputField.text);
        int gpuLayerCount = string.IsNullOrEmpty(gpuLayerCountInputField.text) ? DefaultGpuLayerCount : int.Parse(gpuLayerCountInputField.text);

        float temperature = string.IsNullOrEmpty(temperatureInputField.text) ? DefaultTemperature : float.Parse(temperatureInputField.text);
        int maxTokens = string.IsNullOrEmpty(maxTokensInputField.text) ? DefaultMaxTokens : int.Parse(maxTokensInputField.text);

        string context = DefaultContextPath + "/" + contextDropdown.options[contextDropdown.value].text;

        return new() { Name = presetName, Value = new ChatParams(modelName, (ExecutorType)executorDropdown.value, contextSize, gpuLayerCount, temperature, maxTokens, context) };
    }
}