using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler
{
    class JobProcessor : IJobVisitor
    {
        readonly IPackageDownloader _downloader;
        readonly IOutJobQueue _outJobQueue;
        readonly IPackageRepository _packageRepository;
        readonly Dictionary<PackageId, PackageLastVersion> _crawlingPackages;

        internal JobProcessor(
            IPackageDownloader downloader,
            IOutJobQueue outJobQueue,
            IPackageRepository packageRepository )
        {
            Debug.Assert( downloader != null );
            Debug.Assert( outJobQueue != null );
            Debug.Assert( packageRepository != null );

            _downloader = downloader;
            _outJobQueue = outJobQueue;
            _packageRepository = packageRepository;
            _crawlingPackages = new Dictionary<PackageId, PackageLastVersion>();
        }

        internal async Task Execute( IJob job )
        {
            await job.Accept( this );
        }

        public async Task Visit( VPackageCrawledJob job )
        {
            IEnumerable<PackageId> impactedPackages =
                _crawlingPackages.Where( kv => kv.Value.Contains( job.VPackageId ) )
                                 .Select( kv => kv.Key )
                                 .ToList();

            foreach( PackageId packageId in impactedPackages )
            {
                PackageLastVersion lastVersion = _crawlingPackages[ packageId ];
                lastVersion.PackageFound( job.VPackageId );
                if( lastVersion.IsReady ) await FinishCrawling( lastVersion );
            }
        }

        async Task FinishCrawling( PackageLastVersion lastVersion )
        {
            _crawlingPackages.Remove( lastVersion.PackageId );
            VPackage lastRelease = lastVersion.LastRelease == null ? null : await _packageRepository.GetVPackageById( lastVersion.LastRelease );
            VPackage lastPreRelease = lastVersion.LastPreRelease == null ? null : await _packageRepository.GetVPackageById( lastVersion.LastPreRelease );
            await _packageRepository.AddOrUpdatePackage( new Package( lastVersion.PackageId.Id, lastRelease, lastPreRelease ) );
            await _outJobQueue.PutJob( new PackageCrawledJob( lastVersion.PackageId ) );
        }

        public async Task Visit( CrawlVPackageJob job )
        {
            if( await VPackageUpToDateGuard( job.VPackageId ) ) return;
            PackageInfo packageInfo = await _downloader.GetPackage( job.VPackageId );
            if( packageInfo == null )
            {
                await _packageRepository.AddOrUpdateVPackage( new VPackage( job.VPackageId ) );
                await _outJobQueue.PutJob( new VPackageCrawledJob( job.VPackageId ) );
                return;
            }

            foreach( VPackageId dependency in packageInfo.Dependencies )
            {
                await _outJobQueue.PutJob( new CrawlPackageJob( new PackageId( dependency.Id ) ) );
            }
            await _packageRepository.AddOrUpdateVPackage( new VPackage( job.VPackageId, packageInfo.Dependencies ) );
            await _outJobQueue.PutJob( new VPackageCrawledJob( job.VPackageId ) );
        }

        async Task<bool> VPackageUpToDateGuard( VPackageId vPackageId )
        {
            return await _packageRepository.ExistsVPackage( vPackageId );
        }

        public async Task Visit( CrawlPackageJob job )
        {
            if( IsCrawling( job.PackageId ) ) return;
            if( await PackageUpToDateGuard( job.PackageId ) ) return;

            await Crawl( job.PackageId );
        }

        async Task Crawl( PackageId packageId )
        {
            PackageMaxVersion maxVersion = await _downloader.GetMaxVersion( packageId );
            if( maxVersion == null )
            {
                await _packageRepository.AddOrUpdatePackage( new Package( packageId.Id ) );
                await _outJobQueue.PutJob( new PackageCrawledJob( packageId ) );
                return;
            }
            VPackageId lastRelease = null;
            if( maxVersion.HasReleaseMaxVersion )
            {
                lastRelease = new VPackageId( packageId.Id, maxVersion.MaxVersion );
                await _outJobQueue.PutJob( new CrawlVPackageJob( lastRelease ) );
            }
            VPackageId lastPreRelease = null;
            if( maxVersion.HasPreReleaseMaxVersion )
            {
                lastPreRelease = new VPackageId( packageId.Id, maxVersion.PreReleaseMaxVersion );
                await _outJobQueue.PutJob( new CrawlVPackageJob( lastPreRelease ) );
            }
            PackageLastVersion plv = new PackageLastVersion( packageId, lastRelease, lastPreRelease );
            _crawlingPackages[ packageId ] = plv;
        }

        async Task<bool> PackageUpToDateGuard( PackageId packageId )
        {
            Package package = await _packageRepository.GetPackageById( packageId );
            if( package == null || package.IsNotFound ) return false;
            PackageMaxVersion maxVersion = await _downloader.GetMaxVersion( packageId );
            if( maxVersion == null ) return false;

            return ( ( !maxVersion.HasPreReleaseMaxVersion && package.LastPreRelease == null ) || ( maxVersion.MaxVersion == package.LastPreRelease.VPackageId.Version ) )
                && ( ( !maxVersion.HasReleaseMaxVersion && package.LastRelease == null ) || ( maxVersion.PreReleaseMaxVersion == package.LastRelease.VPackageId.Version ) );
        }

        bool IsCrawling( PackageId packageId )
        {
            return _crawlingPackages.ContainsKey( packageId );
        }

#pragma warning disable 1998

        public async Task Visit( PackageCrawledJob job )
        {
        }

        public async Task Visit( StopJob stopJob )
        {
        }

#pragma warning restore 1998

        class PackageLastVersion
        {
            bool _lastReleaseFound;
            bool _lastPreReleaseFound;

            public PackageLastVersion( PackageId packageId, VPackageId lastRelease, VPackageId lastPreRelease )
            {
                PackageId = packageId;
                LastRelease = lastRelease;
                LastPreRelease = lastPreRelease;
                _lastReleaseFound = LastRelease == null;
                _lastPreReleaseFound = LastPreRelease == null;
            }

            public PackageId PackageId { get; }

            public VPackageId LastRelease { get; }

            public VPackageId LastPreRelease { get; }

            public bool Contains( VPackageId vPackageId )
            {
                return AreEqual( LastPreRelease, vPackageId ) || AreEqual( LastRelease, vPackageId );
            }

            public void PackageFound( VPackageId vPackageId )
            {
                if( AreEqual( LastRelease, vPackageId ) ) _lastReleaseFound = true;
                if( AreEqual( LastPreRelease, vPackageId ) ) _lastPreReleaseFound = true;
            }

            public bool IsReady => _lastReleaseFound && _lastPreReleaseFound;

            bool AreEqual( VPackageId vPackageId1, VPackageId vPackageId2 )
            {
                return vPackageId1 != null && vPackageId1.Equals( vPackageId2 );
            }
        }

    }
}
