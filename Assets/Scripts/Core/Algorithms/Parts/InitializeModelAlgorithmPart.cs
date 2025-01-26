using System.Threading;
using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class InitializeModelAlgorithmPart : IAlgorithmPart
    {
        private readonly string _modelVariableName;
        private readonly ChatParams _chatParams;

        public InitializeModelAlgorithmPart(string modelVariableName, ChatParams chatParams) => (this._modelVariableName, this._chatParams) = (modelVariableName, chatParams);

        public async Task Execute(AlgorithmContext context)
        {
            // Initializing a model
            NeuralNetworkModel neuralNetworkModel = new(_chatParams);

            // Creating a TaskCompletionSource to manage the completion state
            TaskCompletionSource<bool> taskCompletionSource = new();
            
            // Creating new CancellationTokenSource to manage releasing
            CancellationTokenSource cancellationTokenSource = new();

            // Starting the initialization in a separate task
            _ = neuralNetworkModel.Initialize(context.CancellationTokenSource, cancellationTokenSource, taskCompletionSource);

            // Waiting for the initialization to complete
            await taskCompletionSource.Task;

            // Assigning model to the variables
            context.Variables[_modelVariableName] = new AlgorithmVariable((neuralNetworkModel, cancellationTokenSource));
        }
    }
}
