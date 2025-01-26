using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;
using Unity.VisualScripting;

using Invisi.Pseudocode.Compiler;

public class Compiler : MonoBehaviour, IDisposable
{
    #region Events
    
    public event Action<bool> OnCompilation;
    
    #endregion
    
    [SerializeField] private List<GameObject> compilerMarkPrefabs = new();
    
    private CompilerContext _compilerContext;
    
    private CancellationTokenSource _cancellationTokenSource;
    private TaskCompletionSource<bool> _taskCompletionSource;
    private readonly SemaphoreSlim _semaphore = new(2, 2);
    
    private void Awake() => FindFirstObjectByType<AlgorithmPartBlockFactory>().OnAlgorithmChanged += () => Compile();

    public async Task Compile()
    {
        // Returning control
        await Task.Yield();
        
        // Waiting to enter the semaphore
        await _semaphore.WaitAsync();
        
        // Forcing previous compilation to stop
        _cancellationTokenSource?.Cancel();
        if (_taskCompletionSource != null) await _taskCompletionSource.Task;
        
        // Starting a new compilation
        _cancellationTokenSource = new();
        _taskCompletionSource = new();
        
        // Creating a context
        _compilerContext = new();
        
        // Invoking the event
        OnCompilation?.Invoke(true);
        
        // Handling marks
        for (int i = 0; i < this.transform.childCount; i++)
        {
            // Checking for cancellation
            if (_cancellationTokenSource.Token.IsCancellationRequested) break;
            
            // Getting components
            Transform child = this.transform.GetChild(i);
            CompilerMark mark = child.TryGetComponent(out CompilerMark compilerMark) ? compilerMark : child.AddComponent<CompilerMark>();
            
            // Marking as pending
            mark.Mark(compilerMarkPrefabs[(int)CompilationResult.Pending]);
        }

        // Compiling algorithm
        _compilerContext.LastIndex = this.transform.childCount;
        CompilationResult result = CompilationResult.Pending;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            // Checking for cancellation
            if (_cancellationTokenSource.Token.IsCancellationRequested) break;
            
            // Updating index
            _compilerContext.CurrentIndex = i + 1;
            
            // Getting components
            Transform child = this.transform.GetChild(i);
            CompilerMark mark = child.GetComponent<CompilerMark>();
            
            // Marking as unkown
            mark.Mark(compilerMarkPrefabs[(int)CompilationResult.Unknown]);
            
            // Trying to get compilable
            if (!child.TryGetComponent(out ICompilable compilable)) continue;
            
            // Compiling the result
            result = await compilable.Compile(_compilerContext);
            
            // Marking as result
            mark.Mark(compilerMarkPrefabs[(int)result]);
            
            // Ending the compilation process if an error is encountered
            if (result == CompilationResult.Error) break;
        }
        
        // Invoking the event
        OnCompilation?.Invoke(result == CompilationResult.Success);
        
        // Completing task
        _taskCompletionSource.SetResult(true);
        
        // Releasing the semaphore
        _semaphore.Release();
    }
    
    public void Dispose() => OnDestroy();
    private void OnDestroy()
    {
        _semaphore?.Dispose();
        _cancellationTokenSource?.Cancel();
    }
}

public enum CompilationResult
{
    Pending,
    Unknown,
    Warning,
    Error,
    Success
}