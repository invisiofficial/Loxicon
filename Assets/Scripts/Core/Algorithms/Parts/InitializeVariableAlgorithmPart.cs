using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class InitializeVariableAlgorithmPart : IAlgorithmPart
    {
        private readonly string _variableName;

        public InitializeVariableAlgorithmPart(string variableName) => this._variableName = variableName;

        public async Task Execute(AlgorithmContext context) => context.Variables[_variableName] = new AlgorithmVariable(null);
    }
}