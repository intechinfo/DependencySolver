using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;
using NuGet;

namespace Invenietis.DependencyCrawler.IO
{
    public class NuGetDownloader : IPackageDownloader
    {
        public NuGetDownloader( IFeedProvider feedProvider )
        {
            FeedProvider = feedProvider;
        }

        public IFeedProvider FeedProvider { get; }

        public async Task<PackageMaxVersion> GetMaxVersion( PackageId packageInfo )
        {
            List<IPackage> packages = await Task.Factory.StartNew( () =>
                PackageRepository
                    .GetPackages()
                    .Where( p => p.Id == packageInfo.Value )
                    .ToList()
                    .OrderByDescending( p => p.Version )
                    .ToList() );

            IPackage last = packages.First();
            if( last.IsReleaseVersion() ) return new PackageMaxVersion( last.Version.ToString() );

            IPackage lastRelease = packages.FirstOrDefault( p => p.IsReleaseVersion() );
            if( lastRelease == null ) return new PackageMaxVersion( string.Empty, last.Version.ToString() );

            return new PackageMaxVersion( lastRelease.Version.ToString(), last.Version.ToString() );
        }

        public async Task<PackageInfo> GetPackage( VPackageId vPackageInfo )
        {
            IPackage package = await Task.Factory.StartNew( () =>
            {
                SemanticVersion version = new SemanticVersion( vPackageInfo.Version );
                return PackageRepository
                    .GetPackages()
                    .Where( p => p.Id == vPackageInfo.Id )
                    .ToList()
                    .FirstOrDefault( p => p.Version == version );
            } );

            if( package == null ) return null;

            var dependencies = package.DependencySets
                .Select( s => new
                {
                    PlatformId = new PlatformId( s.TargetFramework.FullName ),
                    VPackageIds = s.Dependencies.Select( d => new VPackageId( PackageId.NuGet, d.Id, d.VersionSpec.MinVersion.ToString() ) )
                } )
                .ToDictionary( x => x.PlatformId, x => x.VPackageIds );

            return new PackageInfo( vPackageInfo, dependencies );
        }

        NuGet.IPackageRepository PackageRepository
        {
            get
            {
                return new AggregateRepository( new DataServicePackageRepositoryFactory(), FeedProvider.Feeds, true );
            }
        }

        class DataServicePackageRepositoryFactory : IPackageRepositoryFactory
        {
            public NuGet.IPackageRepository CreateRepository( string packageSource )
            {
                return new DataServicePackageRepository( new Uri( packageSource ) );
            }
        }

        class PackageDependencyEqualityComparer : IEqualityComparer<PackageDependency>
        {
            public bool Equals( PackageDependency x, PackageDependency y )
            {
                return x.Id == y.Id && x.VersionSpec.MinVersion == y.VersionSpec.MinVersion;
            }

            public int GetHashCode( PackageDependency obj )
            {
                return obj.Id.GetHashCode() << 7 ^ obj.VersionSpec.MinVersion.GetHashCode();
            }
        }
    }
}
