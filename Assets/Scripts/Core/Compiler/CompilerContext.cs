using System.Collections.Generic;

namespace Invisi.Pseudocode.Compiler
{
    public class CompilerContext
    {
        public readonly List<string> Variables = new();
        
        public int CurrentIndex = 0;
        
        public int LastIndex = -1;
    }
}
