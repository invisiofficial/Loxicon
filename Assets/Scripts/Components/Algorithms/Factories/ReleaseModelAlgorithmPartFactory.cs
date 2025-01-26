using System;
using System.Threading.Tasks;

using Invisi.Pseudocode;
using Invisi.Pseudocode.Compiler;

public class ReleaseModelAlgorithmPartFactory : ReleaseAlgorithmPartFactory<string>, IAlgorithmPartFactory, ICompilable
{
    #region Events
    
    public event Action OnCompilationRequired;
    
    #endregion
    
    public IAlgorithmPart Create() => new ReleaseModelAlgorithmPart(_initializedName);
    
    public async Task<CompilationResult> Compile(CompilerContext context)
    {        
        // Simulating work
        context.Variables.Remove(_initializedName);
        
        // Returing success
        return CompilationResult.Success;
    }
}
