using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class InputAlgorithmPart : IAlgorithmPart
    {
        private readonly string _inputVariableName;
        
        private string _message;

        public InputAlgorithmPart(string inputVariableName) => this._inputVariableName = inputVariableName;

        public async Task Execute(AlgorithmContext context)
        {            
            // Clearing message
            _message = string.Empty;
            
            // Listening to a input event
            UserInput.Instance.OnSubmit += Receiver;
            
            // Activating input
            UserInput.Instance.Activate();

            // Waiting for input
            while (!context.CancellationTokenSource.Token.IsCancellationRequested && _message == string.Empty) await Task.Yield();
            if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
            
            // Unlistening to a input event
            UserInput.Instance.OnSubmit -= Receiver;

            // Assigning new variable value            
            AlgorithmVariable algorithmVariable = context.Variables[_inputVariableName.TrimStart('@')] as AlgorithmVariable;
            while (!context.CancellationTokenSource.Token.IsCancellationRequested && !algorithmVariable.Free) await Task.Yield();
            if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
            algorithmVariable.Value = _message;
            
            // Input handler
            void Receiver(string message) => _message = message;
        }
    }
}
