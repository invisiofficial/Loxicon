using Invisi.Pseudocode;

public class ReleaseVariableAlgorithmPartFactory : ReleaseAlgorithmPartFactory<string>, IAlgorithmPartFactory
{
    public IAlgorithmPart Create() => new ReleaseVariableAlgorithmPart(_initializedName);
}
