using System.Threading.Tasks;

namespace Invisi.Pseudocode
{
    public interface IAlgorithmPart
    {
        public Task Execute(AlgorithmContext context);
    }
}