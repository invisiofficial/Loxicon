using System;
using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public class Algorithm : IDisposable
    {
        private readonly IAlgorithmPart[] _algorithmParts;
        
        private AlgorithmContext _algorithmContext;
        
        public Algorithm(IAlgorithmPart[] algorithmParts) => _algorithmParts = algorithmParts;
        
        public async Task Execute()
        {
            // Creating a context
            _algorithmContext = new();
            
            // Executing algorithm parts
            while (_algorithmContext.CurrentIndex < _algorithmParts.Length && !_algorithmContext.CancellationTokenSource.Token.IsCancellationRequested) await _algorithmParts[_algorithmContext.CurrentIndex++].Execute(_algorithmContext);
        }
        
        public void Dispose() => _algorithmContext.CancellationTokenSource?.Cancel();
    }
}