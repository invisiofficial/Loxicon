using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;

using LLama;
using LLama.Common;

using Cysharp.Threading.Tasks;

public class AssistantChat : IDisposable
{
    #region Events

    public event Action OnGenerationStarted;
    public event Action OnGenerationEnded;

    #endregion

    // TODO: Put this in json files
    private readonly List<string> AntiPromts = new() { "User:", "user:", "USER:",
                                                       "Human:", "human:", "HUMAN:" };
    private readonly List<string> RoleNames = new() { "User:", "user:", "USER:",
                                                      "Human:", "human:", "HUMAN:",
                                                      "Assistant:", "assistant:", "ASSISTANT:",
                                                      "Assist:", "assist:", "ASSIST:",
                                                      "Ai:", "ai:", "AI:",
                                                      "System:", "system:", "SYSTEM:"};

    private readonly AssistantParams _assistantParams;

    private ChatSession _chatSession;
    private string _message = string.Empty;

    private bool _isAvailable = true;

    private CancellationTokenSource _cts;

    public AssistantChat(AssistantParams assistantParams) => _assistantParams = assistantParams;

    public async UniTask Inference()
    {
        // Creating cancellation token
        _cts = new();

        // Listening to the conversation
        ConversationManager.Instance.OnMessageReceived += SetMessage;
        ConversationManager.Instance.OnTurnChanged += SetAvailable;

        string modelPath = Application.streamingAssetsPath + "/" + _assistantParams.ModelName;

        // Load weights into memory
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = _assistantParams.ContextSize,
            GpuLayerCount = _assistantParams.GpuLayerCount
        };
        await UniTask.SwitchToThreadPool();
        using var model = LLamaWeights.LoadFromFile(parameters);
        await UniTask.SwitchToMainThread();
        using var context = model.CreateContext(parameters);

        // Creating executor
        LLama.Abstractions.ILLamaExecutor executor = _assistantParams.ExecutorType switch
        {
            //ExecutorType.StatelessExecutor => new StatelessExecutor(model, parameters),
            _ => new InteractiveExecutor(context),
        };

        // Reading chat history
        History history = JsonUtility.FromJson<History>(File.ReadAllText(Application.streamingAssetsPath + "/" + _assistantParams.Context));

        // Adding chat history
        var chatHistory = new ChatHistory();
        foreach (Context historyContext in history.Contexts) chatHistory.AddMessage((AuthorRole)Enum.Parse(typeof(AuthorRole), historyContext.Type), historyContext.Content);

        // Creating chat
        _chatSession = new(executor, chatHistory);

        // Starting chat
        await InferenceRoutine();
    }

    private async UniTask InferenceRoutine()
    {
        var userMessage = string.Empty;
        while (!_cts.Token.IsCancellationRequested)
        {
            // Waiting for input text or cancellation
            await UniTask.WaitUntil(() => _message != string.Empty || _cts.Token.IsCancellationRequested);
            if (_cts.Token.IsCancellationRequested) return;

            userMessage = _message;
            _message = string.Empty;

            // Setting up inference params
            InferenceParams inferenceParams = new()
            {
                Temperature = _assistantParams.Temperature,
                MaxTokens = _assistantParams.MaxTokens,
                AntiPrompts = AntiPromts
            };

            // Invoking event
            OnGenerationStarted?.Invoke();

            // Getting response
            StringBuilder responseBuilder = new();
            await foreach (var text in _chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, userMessage), inferenceParams))
            {
                if (_cts.Token.IsCancellationRequested) return;

                responseBuilder.Append(text);

                await UniTask.NextFrame();
            }

            // Invoking event
            OnGenerationEnded?.Invoke();

            // Showing message
            ConversationManager.Message(ClearResponse(responseBuilder.ToString()));
        }

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

    private void SetMessage(string message) => _message = _isAvailable ? message : string.Empty;
    private void SetAvailable(int turn) => _isAvailable = turn == 0;

    public void Dispose() => _cts?.Cancel();
}

public enum ExecutorType
{
    InteractiveExecutor,
    //StatelessExecutor
}

public class AssistantParams
{
    public readonly string ModelName;

    public readonly ExecutorType ExecutorType;

    public readonly uint? ContextSize;
    public readonly int GpuLayerCount;

    public readonly float Temperature;
    public readonly int MaxTokens;

    public readonly string Context;

    public AssistantParams(string modelName, ExecutorType executorType, uint? contextSize, int gpuLayerCount, float temperature, int maxTokens, string context)
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
