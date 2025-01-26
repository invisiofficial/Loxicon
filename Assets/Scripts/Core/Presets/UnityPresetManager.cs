using System.Collections.Generic;

using UnityEngine;

public class UnityPresetManager<T> : PresetManager<T>
{
    private readonly string _storageKey;

    public UnityPresetManager(string storageKey) => _storageKey = storageKey;

    public override void Save()
    {
        string json = JsonUtility.ToJson(new PresetListWrapper<T>() { Presets = Presets });
        PlayerPrefs.SetString(_storageKey, json);
        PlayerPrefs.Save();
    }

    public override void Load()
    {
        Presets.Clear();

        if (!PlayerPrefs.HasKey(_storageKey)) return;

        string json = PlayerPrefs.GetString(_storageKey);
        PresetListWrapper<T> presetWrapper = JsonUtility.FromJson<PresetListWrapper<T>>(json);
        if (presetWrapper?.Presets != null) Presets.AddRange(presetWrapper.Presets);
    }

    [System.Serializable]
    private class PresetListWrapper<U>
    {
        public List<Preset<U>> Presets;
    }
}