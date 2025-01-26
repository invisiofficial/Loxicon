using System;
using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class OutputAlgorithmPart : IAlgorithmPart
    {
        private readonly string _outputVariableName;

        public OutputAlgorithmPart(string outputVariableName) => this._outputVariableName = outputVariableName;

        public async Task Execute(AlgorithmContext context)
        {            
            // Creating message block
            Action<string> updateMessage = ConversationManager.Message(1);
            
            // Getting output variable
            AlgorithmVariable algorithmVariable = context.Variables[_outputVariableName.TrimStart('@')] as AlgorithmVariable;
            
            // Value handler
            void ValueChangedHandler() => updateMessage.Invoke(algorithmVariable.Value.ToString());
            
            // Listening to the event
            algorithmVariable.OnValueChanged += ValueChangedHandler;
            
            while (!context.CancellationTokenSource.Token.IsCancellationRequested && !algorithmVariable.Free) await Task.Yield();
            if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
            
            // Unlistening to the event
            algorithmVariable.OnValueChanged -= ValueChangedHandler;
            
            // Performing output
            updateMessage.Invoke(algorithmVariable.Value.ToString());
        }
    }
}
