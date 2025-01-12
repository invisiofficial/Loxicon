using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;

using LLama;
using LLama.Common;
using LLama.Sampling;

using Cysharp.Threading.Tasks;

public class NeuralNetworkModel
{
    #region Events
    
    public event Action OnGenerationStarted;
    public event Action<string> OnGeneration;
    public event Action<string> OnGenerationEnded;
    
    #endregion
    
    private readonly List<string> AntiPromts = new() { "User:", "user:", "USER:",
                                                       "Human:", "human:", "HUMAN:" };
    private readonly List<string> RoleNames = new() { "User:", "user:", "USER:",
                                                      "Human:", "human:", "HUMAN:",
                                                      "Assistant:", "assistant:", "ASSISTANT:",
                                                      "Assist:", "assist:", "ASSIST:",
                                                      "Ai:", "ai:", "AI:",
                                                      "System:", "system:", "SYSTEM:"};

    private readonly ChatParams _chatParams;

    private ChatSession _chatSession;

    public NeuralNetworkModel(ChatParams modelParams) => _chatParams = modelParams;

    public async UniTask Initialize(CancellationTokenSource globalCancellationTokenSource, CancellationTokenSource localCancellationTokenSource, TaskCompletionSource<bool> taskCompletionSource)
    {        
        string modelPath = Application.streamingAssetsPath + "/" + _chatParams.ModelName;

        // Load weights into memory
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = _chatParams.ContextSize,
            GpuLayerCount = _chatParams.GpuLayerCount
        };
        await UniTask.SwitchToThreadPool();
        using var model = LLamaWeights.LoadFromFile(parameters);
        await UniTask.SwitchToMainThread();
        using var context = model.CreateContext(parameters);

        // Creating executor
        LLama.Abstractions.ILLamaExecutor executor = _chatParams.ExecutorType switch
        {
            //ExecutorType.StatelessExecutor => new StatelessExecutor(model, parameters),
            _ => new InteractiveExecutor(context),
        };

        // Reading chat history
        History history = JsonUtility.FromJson<History>(File.ReadAllText(Application.streamingAssetsPath + "/" + _chatParams.Context));

        // Adding chat history
        var chatHistory = new ChatHistory();
        foreach (Context historyContext in history.Contexts) chatHistory.AddMessage((AuthorRole)Enum.Parse(typeof(AuthorRole), historyContext.Type), historyContext.Content);

        // Creating chat
        _chatSession = new(executor, chatHistory);
        _chatSession.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(RoleNames));
        
        // Returning control to caller
        taskCompletionSource.SetResult(true);
        
        // Stalling to prevent disposal
        while (!globalCancellationTokenSource.Token.IsCancellationRequested && !localCancellationTokenSource.Token.IsCancellationRequested) await UniTask.NextFrame();
    }

    public async UniTask<string> Infer(string inputText, CancellationTokenSource cancellationTokenSource)
    {
        // Setting up inference params
        InferenceParams inferenceParams = new()
        {
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = _chatParams.Temperature
            },
            MaxTokens = _chatParams.MaxTokens,
            AntiPrompts = AntiPromts
        };
        
        // Invoking event
        OnGenerationStarted?.Invoke();

        // Getting response
        string response = string.Empty;
        await foreach (var text in _chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, inputText), inferenceParams))
        {
            if (cancellationTokenSource.Token.IsCancellationRequested) return null;

            // Updating response
            response += text;
            
            // Invoking event
            OnGeneration?.Invoke(text);

            await UniTask.NextFrame();
        }
        
        // Getting cleared result
        string result = ClearResponse(response.ToString());
        
        // Invoking event
        OnGenerationEnded?.Invoke(result);
        
        // Returning response
        return result;
        
        string ClearResponse(string response)
        {
            // Clearing role names
            foreach (string role in RoleNames) response = response.Replace(role, string.Empty);

            // Trimming response
            response = response.Trim().Trim('\n');

            // Returning cleared response
            return response;
        }
    }
}

public enum ExecutorType
{
    InteractiveExecutor,
    //StatelessExecutor
}

[Serializable]
public struct ChatParams
{
    public string ModelName;

    public ExecutorType ExecutorType;

    public uint ContextSize;
    public int GpuLayerCount;

    public float Temperature;
    public int MaxTokens;

    public string Context;

    public ChatParams(string modelName, ExecutorType executorType, uint contextSize, int gpuLayerCount, float temperature, int maxTokens, string context)
    {
        ModelName = modelName;

        ExecutorType = executorType;

        ContextSize = contextSize;
        GpuLayerCount = gpuLayerCount;

        Temperature = temperature;
        MaxTokens = maxTokens;

        Context = context;
    }
}

[Serializable]
public class History
{
    public List<Context> Contexts = new();
}

[Serializable]
public struct Context
{
    public string Type;
    public string Content;

    public Context(string type, string content)
    {
        Type = type;
        Content = content;
    }
}