using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IInJobQueue
    {
        Task<IJob> PeekNextJob();

        Task<IJob> TakeNextJob();
    }
}