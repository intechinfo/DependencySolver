using System.Collections.Generic;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IPackageRepository
    {
        Task<IEnumerable<PackageId>> GetAllPackageIds();

        Task<IEnumerable<PackageId>> GetPackageIds( PackageSegment segment );

        Task<IEnumerable<VPackageId>> GetVPackageIds( PackageSegment segment );

        Task<IEnumerable<VPackageId>> GetNotCrawledVPackageIds( PackageSegment segment );

        Task<bool> AddIfNotExists( VPackageId vPackageId );

        Task<bool> AddIfNotExists( PackageId vPackageId );

        Task UpdateLastRelease( PackageId id, VPackageId lastReleaseId );

        Task UpdateLastPreRelease( PackageId id, VPackageId lastPreReleaseId );

        Task<IEnumerable<Package>> GetAllPackages();

        Task AddDependenciesIfNotExists( VPackageId vPackageId, IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>> dependencies );

        Task<Package> GetPackageById( PackageId packageId );

        Task<IReadOnlyDictionary<PackageId, CombinedVPackageId>> GetLastVersionsOf(IEnumerable<PackageId> PackageIds);

        Task<bool> AddValidateNodes( PackageId package, VPackageId node);

        Task<bool> RemoveValidateNodes( PackageId package, VPackageId node );

        Task<string> GetValidateNodes( PackageId package );
    }
}
