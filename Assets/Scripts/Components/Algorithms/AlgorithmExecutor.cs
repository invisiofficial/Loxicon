using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using Invisi.Pseudocode;    

[RequireComponent(typeof(AlgorithmFactory))]
public class AlgorithmExecutor : MonoBehaviour, IDisposable
{
    private AlgorithmFactory _algorithmFactory;
    
    private Algorithm _algorithm;
    
    private void Awake() => _algorithmFactory = this.GetComponent<AlgorithmFactory>();
    
    public async void Execute()
    {
        // Creating algorithm
        _algorithm = _algorithmFactory.Create();
        
        // Executing algorithm
        await _algorithm.Execute();
        /*try
        {
            
        }
        catch (Exception e)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }*/
    }
    
    public void Dispose() => OnDestroy();
    private void OnDestroy() => _algorithm?.Dispose();
}
