using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;

using LLama;
using LLama.Common;

using Cysharp.Threading.Tasks;

public class AssistantInput : MonoBehaviour
{
    #region Singleton implementation

    public static AssistantInput Instance;
    private void Awake() => Instance = this;

    #endregion

    #region Events
    
    public static event Action OnGenerationStarted;
    public static event Action OnGenerationEnded;
    
    #endregion

    private readonly List<string> AntiPromts = new() { "User:", "user:", "USER:",
                                                       "Human:", "human:", "HUMAN:" };
    private readonly List<string> RoleNames = new() { "User:", "user:", "USER:",
                                                      "Human:", "human:", "HUMAN:",
                                                      "Assistant:", "assistant:", "ASSISTANT:",
                                                      "Assist:", "assist:", "ASSIST:",
                                                      "Ai:", "ai:", "AI:",
                                                      "System:", "system:", "SYSTEM:"};

    private InferenceParams _inferenceParams;

    private ChatSession _chatSession;
    private string _message = string.Empty;

    private bool _isAvailable = true;

    private CancellationTokenSource _cts;

    public static async UniTask Initialize(AssistantParams assistantParams) => await Instance.OnInitialize(assistantParams);
    private async UniTask OnInitialize(AssistantParams assistantParams)
    {
        // Creating cancellation token
        _cts = new();
        
        // Unlistening to the conversation
        ConversationHandler.OnMessageReceived -= SetMessage;
        ConversationHandler.OnTurnChanged -= SetAvailable;
        
        // Listening to the conversation
        ConversationHandler.OnMessageReceived += SetMessage;
        ConversationHandler.OnTurnChanged += SetAvailable;

        string modelPath = Application.streamingAssetsPath + "/" + assistantParams.ModelName;

        // Load weights into memory
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = assistantParams.ContextSize,
            GpuLayerCount = assistantParams.GpuLayerCount
        };
        await UniTask.SwitchToThreadPool();
        using var model = LLamaWeights.LoadFromFile(parameters);
        await UniTask.SwitchToMainThread();
        using var context = model.CreateContext(parameters);
        
        // Creating executor
        LLama.Abstractions.ILLamaExecutor executor = assistantParams.ExecutorType switch
        {
            //ExecutorType.StatelessExecutor => new StatelessExecutor(model, parameters),
            _ => new InteractiveExecutor(context),
        };

        // Adding chat history
        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System, assistantParams.SystemContext);

        // Creating chat
        _chatSession = new(executor, chatHistory);

        // Setting up inference params
        _inferenceParams = new()
        {
            Temperature = assistantParams.Temperature,
            MaxTokens = assistantParams.MaxTokens,
            AntiPrompts = AntiPromts
        };

        // Starting chat
        await InferenceRoutine();
    }

    private async UniTask InferenceRoutine()
    {
        var userMessage = string.Empty;
        while (!_cts.Token.IsCancellationRequested)
        {
            // Waiting for input text
            await UniTask.WaitUntil(() => _message != string.Empty);
            userMessage = _message;
            _message = string.Empty;
            
            // Invoking event
            OnGenerationStarted?.Invoke();

            // Getting response
            StringBuilder stringBuilder = new();
            await foreach (var text in _chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, userMessage), _inferenceParams))
            {
                if (_cts.Token.IsCancellationRequested) return;
                
                stringBuilder.Append(text);

                await UniTask.NextFrame();
            }
            
            // Invoking event
            OnGenerationEnded?.Invoke();

            // Showing message
            ConversationHandler.Message(ClearResponse(stringBuilder.ToString()));
        }
    }

    private string ClearResponse(string response)
    {
        // Clearing role names
        foreach (string role in RoleNames) response = response.Replace(role, string.Empty);

        // Trimming response
        response = response.Trim().Trim('\n');

        // Returning cleared response
        return response;
    }

    private void SetMessage(string message) => _message = _isAvailable ? message : string.Empty;
    private void SetAvailable(int turn) => _isAvailable = turn == 0;

    public static void ResetState()
    {
        Instance._cts?.Cancel();
        Instance._isAvailable = true;
    }
    private void OnDestroy() => ResetState();
}

public enum ExecutorType
{
    InteractiveExecutor,
    //StatelessExecutor
}

public class AssistantParams
{
    public string ModelName;

    public ExecutorType ExecutorType;

    public uint? ContextSize;
    public int GpuLayerCount;

    public float Temperature;
    public int MaxTokens;

    public string SystemContext;

    public AssistantParams(string modelName, ExecutorType executorType, uint? contextSize, int gpuLayerCount, float temperature, int maxTokens, string systemContext)
    {
        ModelName = modelName;
        
        ExecutorType = executorType;
        
        ContextSize = contextSize;
        GpuLayerCount = gpuLayerCount;
        
        Temperature = temperature;
        MaxTokens = maxTokens;
        
        SystemContext = systemContext;
    }
}
