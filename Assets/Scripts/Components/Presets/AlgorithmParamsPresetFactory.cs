using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class AlgorithmParamsPresetFactory : PresetFactory<AlgorithmParams>
{    
    private const string DefaultPresetName = "Algrthm";
    
    protected override void UpdateFields()
    {
        // Updating preset name
        presetNameInputField.placeholder.GetComponent<TMP_Text>().text = DefaultPresetName;
    }
    
    protected override void UpdatePresets()
    {
        // Clearing buttons
        foreach (Transform child in this.transform) Destroy(child.gameObject);

        // Creating create button
        CreateCreateButton();

        // Creating preset buttons
        IReadOnlyList<Preset<AlgorithmParams>> presets = _presetManager.GetPresets();
        for (int i = 0; i < presets.Count; i++) CreatePresetButton(i, presets[i].Name);
    }
    
    protected override void SetPreset(Preset<AlgorithmParams> preset) { presetNameInputField.text = preset.Name; FindFirstObjectByType<AlgorithmPartBlockFactory>().SetPreset(preset); }

    protected override Preset<AlgorithmParams> GetPreset() => FindFirstObjectByType<AlgorithmPartBlockFactory>().GetPreset(string.IsNullOrEmpty(presetNameInputField.text) ? DefaultPresetName : presetNameInputField.text);
}
