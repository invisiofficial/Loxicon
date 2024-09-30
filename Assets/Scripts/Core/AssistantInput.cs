using System.Text;
using System.Collections.Generic;

using UnityEngine;

using LLama;
using LLama.Common;

using Cysharp.Threading.Tasks;
using System.Threading;

public class AssistantInput : MonoBehaviour
{
    private ChatSession _chatSession;
    private string _message = string.Empty;
    
    private bool _isAvailable = true;
    
    private CancellationTokenSource _cts;

    private async UniTaskVoid Start()
    {
        _cts = new();
        
        // Listening to the conversation
        ConversationHandler.OnMessageReceived += SetMessage;
        ConversationHandler.OnTurnChanged += SetAvailable;
        
        string modelPath = Application.streamingAssetsPath + "/" + "qwen2.5-7b-instruct-q4_k_m.gguf";

        // Load weights into memory
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            GpuLayerCount = 30
        };
        await UniTask.SwitchToThreadPool();
        using var model = LLamaWeights.LoadFromFile(parameters);
        await UniTask.SwitchToMainThread();
        using var context = model.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);

        // Adding chat history
        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System, "You are an all-knowing AI named Qwen, created by Alibaba Cloud. Qwen is a very responsive assistant who always answers accurately with the least words possible. Qwen always uses English in its answers.");
        chatHistory.AddMessage(AuthorRole.User, "Hello, Qwen.");
        chatHistory.AddMessage(AuthorRole.Assistant, "Hello. How may I help you today?");
        chatHistory.AddMessage(AuthorRole.User, "How much is 2 + 2?");
        chatHistory.AddMessage(AuthorRole.Assistant, "Of course! Two plus two (2 + 2) equals four (4)");

        // Creating chat
        _chatSession = new(executor, chatHistory);
        
        // Starting chat
        await InferenceRoutine(_cts.Token);
    }
    
    private async UniTask InferenceRoutine(CancellationToken cancel = default)
    {
        var userMessage = string.Empty;
        while (!cancel.IsCancellationRequested)
        {
            // Waiting for input text
            await UniTask.WaitUntil(() => _message != string.Empty);
            userMessage = _message;
            _message = string.Empty;
            
            // Setting up params
            InferenceParams inferenceParams = new()
            {
                Temperature = 0.6f,
                MaxTokens = 2048,
                AntiPrompts = new List<string> { "User:", "user:", "USER:", "Human:", "human:", "HUMAN:" }
            };
            
            // Getting response
            StringBuilder stringBuilder = new();
            await foreach (var text in _chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, userMessage), inferenceParams))
            {                
                stringBuilder.Append(text);
                
                await UniTask.NextFrame();
            }
            
            // Showing message
            ConversationHandler.Message(stringBuilder.ToString()
                                        .Replace("User:", string.Empty)
                                        .Replace("user:", string.Empty)
                                        .Replace("USER:", string.Empty)
                                        .Replace("Human:", string.Empty)
                                        .Replace("human:", string.Empty)
                                        .Replace("HUMAN:", string.Empty)
                                        .Replace("Assistant:", string.Empty)
                                        .Replace("assistant:", string.Empty)
                                        .Replace("ASSISTANT:", string.Empty)
                                        .Trim()
                                        .Trim('\n'));
        }
    }
    
    public void SetMessage(string message) => _message = _isAvailable ? message : string.Empty;
    private void SetAvailable(int turn) => _isAvailable = turn == 0;
    
    private void OnDestroy() => _cts.Cancel();
}
