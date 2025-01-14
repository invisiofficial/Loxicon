using System;
using System.Threading.Tasks;

namespace Invisi.Pseudocode.Compiler
{
    public interface ICompilable
    {
        #region Events
        
        public event Action OnCompilationRequired;
        
        #endregion
        
        public Task<CompilationResult> Compile(CompilerContext context);
    }
}