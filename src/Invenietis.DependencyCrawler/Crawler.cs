using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler
{
    public class Crawler : ICrawler
    {
        readonly TimeSpan _iterationTimeSpan;
        bool _newItemsFound;

        public Crawler(
            IPackageDownloader packageDownloader,
            IPackageRepository packageRepository,
            PackageSegment segment )
            : this( packageDownloader, packageRepository, segment, TimeSpan.FromMinutes( 10 ) )
        {
        }

        public Crawler(
            IPackageDownloader packageDownloader,
            IPackageRepository packageRepository,
            PackageSegment segment,
            TimeSpan iterationTimeSpan )
        {
            PackageDownloader = packageDownloader;
            PackageRepository = packageRepository;
            Segment = segment;
            _iterationTimeSpan = iterationTimeSpan;
        }

        public IPackageDownloader PackageDownloader { get; }

        public IPackageRepository PackageRepository { get; }

        public PackageSegment Segment { get; }

        public async Task Start()
        {
            for( ;;)
            {
                DateTime lastStepDate = DateTime.UtcNow;
                await Start( 1 );
                TimeSpan stepTimeSpan = DateTime.UtcNow - lastStepDate;
                TimeSpan remaining = _iterationTimeSpan - stepTimeSpan;
                if( remaining > TimeSpan.FromSeconds( 0 ) ) await Task.Delay( remaining );
            }
        }

        public async Task Start( int steps )
        {
            do
            {
                _newItemsFound = false;
                IEnumerable<PackageId> packages = await PackageRepository.GetPackageIds( Segment );
                foreach( PackageId packageId in packages.ToList() ) await Crawl( packageId );
                IEnumerable<VPackageId> notCrawledVPackages = await PackageRepository.GetNotCrawledVPackageIds( Segment );
                foreach( VPackageId vPackageId in notCrawledVPackages.ToList() ) await Crawl( vPackageId );
            } while( _newItemsFound );
        }

        async Task Crawl( VPackageId vPackageId )
        {
            PackageInfo packageInfo = await PackageDownloader.GetPackage( vPackageId );
            foreach( VPackageId dependencyId in packageInfo.Dependencies )
            {
                bool added = await PackageRepository.AddIfNotExists( new PackageId( dependencyId.PackageManager, dependencyId.Id ) );
                _newItemsFound = _newItemsFound || added;
                added = await PackageRepository.AddIfNotExists( dependencyId );
                _newItemsFound = _newItemsFound || added;
            }
            await PackageRepository.AddDependenciesIfNotExists( vPackageId, packageInfo.Dependencies );
        }

        async Task Crawl( PackageId packageId )
        {
            PackageMaxVersion maxVersion = await PackageDownloader.GetMaxVersion( packageId );
            await UpdateMaxVersion( packageId, maxVersion.HasReleaseMaxVersion, maxVersion.ReleaseMaxVersion, PackageRepository.UpdateLastRelease );
            await UpdateMaxVersion( packageId, maxVersion.HasPreReleaseMaxVersion, maxVersion.PreReleaseMaxVersion, PackageRepository.UpdateLastPreRelease );
        }

        async Task UpdateMaxVersion( PackageId packageId, bool hasMaxVersion, string maxVersion, Func<PackageId, VPackageId, Task> update )
        {
            if( hasMaxVersion )
            {
                VPackageId lastReleaseId = new VPackageId( packageId.PackageManager, packageId.Value, maxVersion );
                bool added = await PackageRepository.AddIfNotExists( lastReleaseId );
                _newItemsFound = _newItemsFound || added;
                await update( packageId, lastReleaseId );
            }
        }
    }
}
