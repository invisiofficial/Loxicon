using System.Threading;
using System.Collections.Generic;

namespace Invisi.Pseudocode
{
    public class AlgorithmContext
    {
        public readonly Dictionary<string, object> Variables = new();
        public int CurrentIndex = 0;
        public CancellationTokenSource CancellationTokenSource = new();
    }
}
