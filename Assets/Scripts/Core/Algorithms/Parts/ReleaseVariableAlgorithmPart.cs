using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class ReleaseVariableAlgorithmPart : IAlgorithmPart
    {
        private readonly string _variableName;

        public ReleaseVariableAlgorithmPart(string variableName) => this._variableName = variableName;

        public async Task Execute(AlgorithmContext context)
        {
            // Getting variable
            AlgorithmVariable algorithmVariable = context.Variables[_variableName] as AlgorithmVariable;
            while (!context.CancellationTokenSource.Token.IsCancellationRequested && !algorithmVariable.Free) await Task.Yield();
            if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
            
            // Releasing variable
            context.Variables.Remove(_variableName);
        }
    }
}
