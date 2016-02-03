using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    class FakePackageRepository : IPackageRepository
    {
        readonly Dictionary<string, Package> _packages;
        readonly Dictionary<VPackageId, VPackage> _vPackages;
        IReadOnlyCollection<PackageId> _rootPackages;

        internal FakePackageRepository()
        {
            _packages = new Dictionary<string, Package>();
            _vPackages = new Dictionary<VPackageId, VPackage>();
        }

#pragma warning disable 1998

        public async Task AddOrUpdatePackage( Package package )
        {
            _packages[ package.Name ] = package;
        }

        public async Task AddOrUpdateVPackage( VPackage vPackage )
        {
            _vPackages.Add( vPackage.VPackageId, vPackage );
        }

        public async Task<bool> ExistsVPackage( VPackageId vPackageId )
        {
            return _vPackages.ContainsKey( vPackageId );
        }

        public async Task<IEnumerable<Package>> GetAllPackages()
        {
            return _packages.Values;
        }

        public async Task<IEnumerable<VPackage>> GetAllVPackages()
        {
            return _vPackages.Values;
        }

        public async Task<Package> GetPackageById( PackageId packageId )
        {
            Package result;
            _packages.TryGetValue( packageId.Id, out result );
            return result;
        }

        public async Task<IReadOnlyCollection<PackageId>> GetRootPackages()
        {
            return _rootPackages;
        }

        public async Task<VPackage> GetVPackageById( VPackageId vPackageId )
        {
            VPackage result;
            _vPackages.TryGetValue( vPackageId, out result );
            return result;
        }

#pragma warning restore 1998

        internal void SetRootPackages(IReadOnlyCollection<PackageId> rootPackages)
        {
            _rootPackages = rootPackages;
        }
    }
}