using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IOutJobQueue
    {
        Task PutJob( IJob job );
    }
}