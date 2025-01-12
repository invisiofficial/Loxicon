using System;
using System.Threading;
using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class InferModelAlgorithmPart : IAlgorithmPart
    {
        private readonly string _inputVariableName;
        private readonly string _modelVariableName;
        private readonly string _outputVariableName;

        public InferModelAlgorithmPart(string inputVariableName, string modelVariableName, string outputVariableName) => (this._inputVariableName, this._modelVariableName, this._outputVariableName) = (inputVariableName, modelVariableName, outputVariableName);

        public async Task Execute(AlgorithmContext context)
        {            
            // Checking for variable
            if (!_inputVariableName.StartsWith('@') || !_modelVariableName.StartsWith('@') || !_outputVariableName.StartsWith('@')) throw new ArgumentException($"Incorrect name format! Variable name has to start with '@'.");
            
            // Getting a model variable
            AlgorithmVariable modelAlgorithmVariable = context.Variables[_modelVariableName.TrimStart('@')] as AlgorithmVariable;
            while (!context.CancellationTokenSource.Token.IsCancellationRequested && !modelAlgorithmVariable.Free) await Task.Yield();
            if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
            modelAlgorithmVariable.Free = false;
            
            NeuralNetworkModel neuralNetworkModel = (((NeuralNetworkModel, CancellationTokenSource))modelAlgorithmVariable.Value).Item1;
            
            // Getting inference input
            AlgorithmVariable algorithmVariable = context.Variables[_inputVariableName.TrimStart('@')] as AlgorithmVariable;
            while (!context.CancellationTokenSource.Token.IsCancellationRequested && !algorithmVariable.Free) await Task.Yield();
            if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
            string input = algorithmVariable.Value.ToString();
            
            // Getting inference output
            algorithmVariable = context.Variables[_outputVariableName.TrimStart('@')] as AlgorithmVariable;
            while (!context.CancellationTokenSource.Token.IsCancellationRequested && !algorithmVariable.Free) await Task.Yield();
            if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
            algorithmVariable.Free = false;
            algorithmVariable.Value = string.Empty;

            // Generation handlers
            void GenerationStartedHandler()
            {
                ConversationPrinter.Instance.MentionGenerating(true);
            }

            void GenerationHandler(string part)
            {
                algorithmVariable.Value += part;
            }

            void GenerationEndedHandler(string text)
            {
                ConversationPrinter.Instance.MentionGenerating(false);
                
                algorithmVariable.Value = text;
                algorithmVariable.Free = true;
                
                neuralNetworkModel.OnGenerationStarted -= GenerationStartedHandler;
                neuralNetworkModel.OnGeneration -= GenerationHandler;
                neuralNetworkModel.OnGenerationEnded -= GenerationEndedHandler;
                
                modelAlgorithmVariable.Free = true;
            }

            // Listening to the events
            neuralNetworkModel.OnGenerationStarted += GenerationStartedHandler;
            neuralNetworkModel.OnGeneration += GenerationHandler;
            neuralNetworkModel.OnGenerationEnded += GenerationEndedHandler;
            
            // Performing inference
            _ = neuralNetworkModel.Infer(input, context.CancellationTokenSource);        
        }
    }
}
