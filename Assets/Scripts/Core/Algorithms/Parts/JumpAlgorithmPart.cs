using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class JumpAlgorithmPart : IAlgorithmPart
    {
        private readonly string _conditionVariableName;
        private readonly int _jumpIndex;

        public JumpAlgorithmPart(string conditionVariableName, int jumpIndex) => (this._conditionVariableName, this._jumpIndex) = (conditionVariableName, jumpIndex);

        public async Task Execute(AlgorithmContext context)
        {            
            // Checking for condition
            if (_conditionVariableName == "@false") return;
            if (_conditionVariableName != "@true")
            {
                AlgorithmVariable algorithmVariable = context.Variables[_conditionVariableName.TrimStart('@')] as AlgorithmVariable;
                while (!context.CancellationTokenSource.Token.IsCancellationRequested && !algorithmVariable.Free) await Task.Yield();
                if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
                if (algorithmVariable.Value.ToString() == string.Empty) return;
            }
            
            // Jumping to the new index
            context.CurrentIndex = _jumpIndex - 1;
        }
    }
}