using System.Collections.Generic;

public abstract class PresetManager<T>
{  
    protected readonly List<Preset<T>> Presets = new();

    public IReadOnlyList<Preset<T>> GetPresets() => Presets.AsReadOnly();

    public void AddPreset(Preset<T> preset) => Presets.Add(preset);

    public void UpdatePreset(int index, T preset) => Presets[index].Value = preset;

    public void RemovePreset(int index) => Presets.RemoveAt(index);

    public abstract void Save();
    public abstract void Load();
}

[System.Serializable]
public class Preset<T>
{
    public string Name;
    public T Value;
}