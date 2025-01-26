using System.Threading;
using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class ReleaseModelAlgorithmPart : IAlgorithmPart
    {
        private readonly string _modelVariableName;

        public ReleaseModelAlgorithmPart(string modelVariableName) => this._modelVariableName = modelVariableName;

        public async Task Execute(AlgorithmContext context)
        {
            // Getting variable
            AlgorithmVariable algorithmVariable = context.Variables[_modelVariableName] as AlgorithmVariable;
            while (!context.CancellationTokenSource.Token.IsCancellationRequested && !algorithmVariable.Free) await Task.Yield();
            if (context.CancellationTokenSource.Token.IsCancellationRequested) return;
            
            // Disposing model variable
            CancellationTokenSource cancellationTokenSource = (((NeuralNetworkModel, CancellationTokenSource))algorithmVariable.Value).Item2;
            cancellationTokenSource?.Cancel();
            
            // Removing variable from context
            context.Variables.Remove(_modelVariableName);
        }
    }
}