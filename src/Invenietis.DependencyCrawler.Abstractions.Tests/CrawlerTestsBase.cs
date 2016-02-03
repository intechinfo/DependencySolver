using Invenietis.DependencyCrawler.Core;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    [TestFixture]
    public abstract class CrawlerTestsBase
    {
        //                           +-----------------------+                                           
        //                   /------>| Last release : v1.0.0 |---------------\ 
        // +----------+     /        +-----------------------+                \       +-----------------+
        // | Package1 |-----                                                   ------>| Package2 v2.0.0 |
        // +----------+     \        +-------------------------------+        /       +-----------------+
        //                   \------>| Last prerelease : v1.0.1-beta |-------/
        //                           +-------------------------------+
        // Package2 :
        //  - Last release : v2.0.0
        //  - Last prerelease : none
        [Test]
        public async Task Start_WithSimplePackageGraph_ShouldFillPackageRepositoryCorrectly()
        {
            IPackageDownloader fakeDownloader = Substitute.For<IPackageDownloader>();
            fakeDownloader.GetMaxVersion( new PackageId( "Package1" ) ).Returns( new PackageMaxVersion( "1.0.0", "1.0.1-beta" ) );
            fakeDownloader.GetMaxVersion( new PackageId( "Package2" ) ).Returns( new PackageMaxVersion( "2.0.0" ) );
            fakeDownloader.GetPackage( new VPackageId( "Package1", "1.0.0" ) )
                .Returns( new PackageInfo( new VPackageId( "Package1", "1.0.0" ), new[] { new VPackageId( "Package2", "2.0.0" ) } ) );
            fakeDownloader.GetPackage( new VPackageId( "Package1", "1.0.1-beta" ) )
                .Returns( new PackageInfo( new VPackageId( "Package1", "1.0.1-beta" ), new[] { new VPackageId( "Package2", "2.0.0" ) } ) );
            fakeDownloader.GetPackage( new VPackageId( "Package2", "2.0.0" ) )
                .Returns( new PackageInfo( new VPackageId( "Package2", "2.0.0" ), new VPackageId[ 0 ] ) );

            FakePackageRepository fakeRepository = new FakePackageRepository();
            ICrawlerStateStorage fakeStateStorage = Substitute.For<ICrawlerStateStorage>();
            FakeJobQueue fakeJobQueue = new FakeJobQueue();
            ICrawler sut = CreateCrawler(
                fakeDownloader,
                fakeRepository,
                fakeStateStorage,
                fakeJobQueue,
                fakeJobQueue,
                new PackageSegment( "a", "z" ) );

            await fakeJobQueue.PutJob( new CrawlPackageJob( new PackageId( "Package1" ) ) );

            await sut.Start();

            Assert.That( await fakeRepository.GetAllPackages(), Is.EquivalentTo( new Package[]
            {
                new Package(
                    "Package1",
                        new VPackage(
                            new VPackageId( "Package1", "1.0.0" ),
                            new[] { new VPackageId( "Package2", "2.0.0" ) } ),
                        new VPackage(
                            new VPackageId( "Package1", "1.0.1-beta" ),
                            new[] { new VPackageId( "Package2", "2.0.0" ) } ) ),
                new Package(
                    "Package2",
                        new VPackage(
                            new VPackageId("Package2", "2.0.0" ),
                            new VPackageId[0] ) )
            } ) );
        }

        //                           +-----------------------+                +----------------+
        //                   /------>| Last release : v1.0.0 |--------------->| Package2 2.0.0 |
        // +----------+     /        +-----------------------+                +----------------+
        // | Package1 |-----
        // +----------+     \        +-------------------------------+        +---------------------+
        //                   \------>| Last prerelease : v1.1.0-beta |------->| Package2 2.1.0-beta |
        //                           +-------------------------------+        +---------------------+
        // 
        //                           +-----------------------+
        //                   /------>| Last release : v2.0.1 |
        // +----------+     /        +-----------------------+
        // | Package2 |-----
        // +----------+     \        +-------------------------------+         +----------------+
        //                   \------>| Last prerelease : v2.1.0-beta |-------->| Package3 3.0.0 |
        //                           +-------------------------------+         +----------------+
        //
        // +----------+         +----------------+
        // | Package3 |-------->| Package3 3.1.0 |
        // +----------+         +----------------+
        //
        [Test]
        public async Task Start_WithAnotherSimplePackageGraph_ShouldFillPackageRepositoryCorrectly()
        {
            IPackageDownloader fakeDownloader = Substitute.For<IPackageDownloader>();
            fakeDownloader.GetMaxVersion( new PackageId( "Package1" ) ).Returns( new PackageMaxVersion( "1.0.0", "1.1.0-beta" ) );
            fakeDownloader.GetMaxVersion( new PackageId( "Package2" ) ).Returns( new PackageMaxVersion( "2.0.1", "2.1.0-beta" ) );
            fakeDownloader.GetMaxVersion( new PackageId( "Package3" ) ).Returns( new PackageMaxVersion( "3.1.0" ) );
            fakeDownloader.GetPackage( new VPackageId( "Package1", "1.0.0" ) )
                .Returns( new PackageInfo( new VPackageId( "Package1", "1.0.0" ), new[] { new VPackageId( "Package2", "2.0.0" ) } ) );
            fakeDownloader.GetPackage( new VPackageId( "Package1", "1.1.0-beta" ) )
                .Returns( new PackageInfo( new VPackageId( "Package1", "1.1.0-beta" ), new[] { new VPackageId( "Package2", "2.1.0-beta" ) } ) );
            fakeDownloader.GetPackage( new VPackageId( "Package2", "2.0.1" ) )
                .Returns( new PackageInfo( new VPackageId( "Package2", "2.0.1" ), new VPackageId[ 0 ] ) );
            fakeDownloader.GetPackage( new VPackageId( "Package2", "2.1.0-beta" ) )
                .Returns( new PackageInfo( new VPackageId( "Package2", "2.1.0-beta" ), new[] { new VPackageId( "Package3", "3.0.0" ) } ) );
            fakeDownloader.GetPackage( new VPackageId( "Package3", "3.1.0" ) )
                .Returns( new PackageInfo( new VPackageId( "Package3", "3.1.0" ), new VPackageId[ 0 ] ) );

            FakePackageRepository fakeRepository = new FakePackageRepository();
            ICrawlerStateStorage fakeStateStorage = Substitute.For<ICrawlerStateStorage>();
            FakeJobQueue fakeJobQueue = new FakeJobQueue();
            ICrawler sut = CreateCrawler(
                fakeDownloader,
                fakeRepository,
                fakeStateStorage,
                fakeJobQueue,
                fakeJobQueue,
                new PackageSegment( "a", "z" ) );

            await fakeJobQueue.PutJob( new CrawlPackageJob( new PackageId( "Package1" ) ) );

            await sut.Start();

            Assert.That( await fakeRepository.GetAllPackages(), Is.EquivalentTo( new Package[]
            {
                new Package(
                    "Package1",
                        new VPackage(
                            new VPackageId( "Package1", "1.0.0" ),
                            new[] { new VPackageId( "Package2", "2.0.0" ) } ),
                        new VPackage(
                            new VPackageId( "Package1", "1.1.0-beta" ),
                            new[] { new VPackageId( "Package2", "2.1.0-beta" ) } ) ),
                new Package(
                    "Package2",
                        new VPackage(
                            new VPackageId("Package2", "2.0.1" ),
                            new VPackageId[0] ),
                        new VPackage(
                            new VPackageId("Package2", "2.1.0-beta" ),
                            new[] { new VPackageId( "Package3", "3.0.0" ) } ) ),
                new Package(
                    "Package3",
                        new VPackage(
                            new VPackageId("Package3", "3.1.0" ),
                            new VPackageId[0] ) )
            } ) );
        }

        protected abstract ICrawler CreateCrawler(
            IPackageDownloader downloader,
            IPackageRepository repository,
            ICrawlerStateStorage stateStorage,
            IInJobQueue inJobQueue,
            IOutJobQueue outJobQueue,
            PackageSegment segment );
    }
}
