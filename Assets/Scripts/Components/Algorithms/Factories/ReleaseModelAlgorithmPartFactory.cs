using Invisi.Pseudocode;

public class ReleaseModelAlgorithmPartFactory : ReleaseAlgorithmPartFactory<string>, IAlgorithmPartFactory
{
    public IAlgorithmPart Create() => new ReleaseModelAlgorithmPart(_initializedName);
}
