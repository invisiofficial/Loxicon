using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class AlgorithmPartBlockFactory : MonoBehaviour
{
    [SerializeField] private GameObject[] factoryBlockPrefabs;

    private readonly Dictionary<GameObject, int> _factoryBlockIndexMap = new();
    private readonly List<GameObject> _factoryBlockKeys = new();

    public void Create(int index, string serialization = null)
    {
        // Instantiating the object
        GameObject gameObject = Instantiate(factoryBlockPrefabs[index], this.transform);
        
        // Adding the block to the lists
        _factoryBlockIndexMap.Add(gameObject, index);
        _factoryBlockKeys.Add(gameObject);
        
        // Configuring the view
        Draggable draggable = gameObject.AddComponent<Draggable>();
        
        draggable.OnDelete += () => _factoryBlockIndexMap.Remove(gameObject);
        draggable.OnDelete += () => _factoryBlockKeys.Remove(gameObject);

        // Configuring the model
        if (gameObject.TryGetComponent(out ISerializable serializable) && serialization != null) serializable.Deserialize(serialization);
        
        if (gameObject.TryGetComponent(out IInitializable initializable)) initializable.Initialize(_factoryBlockKeys.ToArray());
        
        // Activating the object
        gameObject.SetActive(true);
    }

    public void SetPreset(Preset<AlgorithmParams> preset)
    {
        // Clearing the lists
        _factoryBlockIndexMap.Clear();
        _factoryBlockKeys.Clear();
        
        // Clearing existing blocks
        foreach (Transform child in transform) Destroy(child.gameObject);

        // Recreating blocks from indices and serialized data
        for (int i = 0; i < preset.Value.FactoryBlockIndices.Count; i++) Create(preset.Value.FactoryBlockIndices[i], preset.Value.FactoryBlockSerializations[i]);
    }

    public Preset<AlgorithmParams> GetPreset(string name)
    {
        AlgorithmParams algorithmParams = new() { FactoryBlockIndices = new(), FactoryBlockSerializations = new() };
        foreach (Transform child in transform)
        {
            if (!child.TryGetComponent(out ISerializable serializable)) continue;
                        
            algorithmParams.FactoryBlockIndices.Add(_factoryBlockIndexMap[child.gameObject]);
            algorithmParams.FactoryBlockSerializations.Add(serializable.Serialize());
        }

        return new Preset<AlgorithmParams> { Name = name, Value = algorithmParams };
    }
}

[Serializable]
public struct AlgorithmParams
{
    public List<int> FactoryBlockIndices;
    public List<string> FactoryBlockSerializations;

    public AlgorithmParams(IEnumerable<int> indices, IEnumerable<string> serializations)
    {
        FactoryBlockIndices = new(indices);
        FactoryBlockSerializations = new(serializations);
    }
}