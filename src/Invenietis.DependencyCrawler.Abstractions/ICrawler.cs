using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface ICrawler
    {
        IPackageDownloader PackageDownloader { get; }

        IPackageRepository PackageRepository { get; }

        PackageSegment Segment { get; }

        Task Start();

        Task Start( int steps );
    }
}
