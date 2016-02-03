using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface ICrawler
    {
        IPackageDownloader PackageDownloader { get; }

        IPackageRepository PackageRepository { get; }

        ICrawlerStateStorage CrawlerStateStorage { get; }

        IInJobQueue InJobQueue { get; }

        IOutJobQueue OutJobQueue { get; }

        PackageSegment Segment { get; }

        Task Start();
    }
}
