using UnityEngine;

public class AlgorithmPartBlockFactoryWorker : MonoBehaviour
{
    [SerializeField] private int[] prefabIndices;
    
    private AlgorithmPartBlockFactory _algorithmPartObjectFactory;
    
    private void Awake() => _algorithmPartObjectFactory = FindFirstObjectByType<AlgorithmPartBlockFactory>();
    
    public void Create() { foreach (int index in prefabIndices) _algorithmPartObjectFactory.Create(index); }
}
