using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IJobQueue : IInJobQueue, IOutJobQueue
    {
        Task Clear();
    }
}
