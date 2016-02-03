using System.Collections.Generic;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IPackageRepository
    {
        Task<bool> ExistsVPackage( VPackageId vPackageId );

        Task AddOrUpdatePackage( Package package );

        Task<Package> GetPackageById( PackageId packageId );

        Task AddOrUpdateVPackage( VPackage vPackage );

        Task<VPackage> GetVPackageById( VPackageId lastPreRelease );

        Task<IReadOnlyCollection<PackageId>> GetRootPackages();
    }
}
