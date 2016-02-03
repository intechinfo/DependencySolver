using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IPackageDownloader
    {
        IFeedProvider FeedProvider { get; }

        Task<PackageInfo> GetPackage( VPackageId package );

        Task<PackageMaxVersion> GetMaxVersion( PackageId p );
    }
}
