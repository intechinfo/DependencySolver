using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    class FakePackageRepository : IPackageRepository
    {
        readonly Dictionary<PackageId, Tuple<VPackageId, VPackageId>> _packages;
        readonly Dictionary<VPackageId, IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>>> _vPackages;

        internal FakePackageRepository()
        {
            _packages = new Dictionary<PackageId, Tuple<VPackageId, VPackageId>>();
            _vPackages = new Dictionary<VPackageId, IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>>>();
        }

#pragma warning disable 1998

        public async Task<bool> AddIfNotExists( PackageId packageId )
        {
            if( !_packages.ContainsKey( packageId ) )
            {
                _packages[ packageId ] = null;
                return true;
            }

            return false;
        }

        public async Task<bool> AddIfNotExists( VPackageId vPackageId )
        {
            if( !_vPackages.ContainsKey( vPackageId ) )
            {
                _vPackages[ vPackageId ] = null;
                return true;
            }

            return false;
        }

        public async Task AddDependenciesIfNotExists( VPackageId vPackageId, IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>> dependencies )
        {
            if( _vPackages[ vPackageId ] == null )
            {
                _vPackages[ vPackageId ] = dependencies;
            }
        }

        public async Task<IEnumerable<Package>> GetAllPackages()
        {
            List<Package> result = new List<Package>();
            foreach( var packageId in _packages.Keys )
            {
                result.Add( GetPackage( packageId ) );
            }

            return result;
        }

        Package GetPackage( PackageId packageId )
        {
            var lastReleases = _packages[ packageId ];
            VPackage lastRelease = GetVPackage( lastReleases.Item1 );
            VPackage lastPreRelease = GetVPackage( lastReleases.Item2 );

            return new Package( packageId, lastRelease, lastPreRelease );
        }

        VPackage GetVPackage( VPackageId packageId )
        {
            if( packageId == null ) return null;

            var dependencies = _vPackages[ packageId ];
            if( dependencies.Count == 0 ) return new VPackage( packageId );

            List<Platform> platforms = new List<Platform>();
            foreach( var platform in dependencies ) platforms.Add( GetPlatform( platform.Key, platform.Value ) );

            return new VPackage( packageId, platforms );
        }

        Platform GetPlatform( PlatformId platformId, IEnumerable<VPackageId> vPackageIds )
        {
            return new Platform( platformId, vPackageIds.Select( p => GetVPackage( p ) ).ToList() );
        }

        public async Task<IEnumerable<VPackageId>> GetNotCrawledVPackageIds( PackageSegment segment )
        {
            return _vPackages.Where( kv => kv.Value == null ).Select( kv => kv.Key );
        }

        public async Task<IEnumerable<PackageId>> GetPackageIds( PackageSegment segment )
        {
            return _packages.Select( kv => kv.Key );
        }

        public async Task UpdateLastRelease( PackageId id, VPackageId lastReleaseId )
        {
            Tuple<VPackageId, VPackageId> currentLastReleases = _packages[ id ];
            _packages[ id ] = Tuple.Create( lastReleaseId, currentLastReleases == null ? null : currentLastReleases.Item2 );
        }

        public async Task UpdateLastPreRelease( PackageId id, VPackageId lastPreReleaseId )
        {
            Tuple<VPackageId, VPackageId> currentLastReleases = _packages[ id ];
            _packages[ id ] = Tuple.Create( currentLastReleases == null ? null : currentLastReleases.Item1, lastPreReleaseId );
        }

        public async Task<IEnumerable<VPackageId>> GetVPackageIds( PackageSegment segment )
        {
            return _vPackages.Keys;
        }

        public async Task<Package> GetPackageById( PackageId packageId )
        {
            return GetPackage( packageId );
        }

#pragma warning restore 1998

    }
}