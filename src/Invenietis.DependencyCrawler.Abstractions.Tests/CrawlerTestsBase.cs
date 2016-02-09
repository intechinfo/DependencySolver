using Invenietis.DependencyCrawler.Core;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            fakeDownloader.GetMaxVersion( new PackageId( PackageId.NuGet, "Package1" ) ).Returns( new PackageMaxVersion( "1.0.0", "1.0.1-beta" ) );
            fakeDownloader.GetMaxVersion( new PackageId( PackageId.NuGet, "Package2" ) ).Returns( new PackageMaxVersion( "2.0.0" ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package1", "1.0.0" ) )
                .Returns( new PackageInfo(
                    new VPackageId( PackageId.NuGet, "Package1", "1.0.0" ),
                    new Dictionary<PlatformId, IEnumerable<VPackageId>> { { PlatformId.None, new[] { new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) } } } ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package1", "1.0.1-beta" ) )
                .Returns( new PackageInfo(
                    new VPackageId( PackageId.NuGet, "Package1", "1.0.1-beta" ),
                    new Dictionary<PlatformId, IEnumerable<VPackageId>> { { PlatformId.None, new[] { new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) } } } ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) )
                .Returns( new PackageInfo( new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) ) );

            FakePackageRepository fakeRepository = new FakePackageRepository();
            await fakeRepository.AddIfNotExists( new PackageId( PackageId.NuGet, "Package1" ) );
            ICrawler sut = CreateCrawler(
                fakeDownloader,
                fakeRepository,
                new PackageSegment( PackageId.NuGet, "a" ) );

            await sut.Start( 1 );

            Assert.That( await fakeRepository.GetAllPackages(), Is.EquivalentTo( new Package[]
            {
                new Package(
                    PackageId.NuGet,
                    "Package1",
                        new VPackage(
                            new VPackageId( PackageId.NuGet, "Package1", "1.0.0" ),
                            new[] { new VPackage( new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) ) } ),
                        new VPackage(
                            new VPackageId( PackageId.NuGet, "Package1", "1.0.1-beta" ),
                            new[] { new VPackage( new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) ) } ) ),
                new Package(
                    PackageId.NuGet,
                    "Package2",
                        new VPackage( new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) ) )
            } ) );
        }

        //                           +-----------------------+                +----------------+
        //                   /------>| Last release : v1.0.0 |--------------->| Package2 2.0.0 |
        // +----------+     /        +-----------------------+                +----------------+
        // | Package1 |-----
        // +----------+     \        +-------------------------------+        +---------------------+         +----------------+         +----------------+
        //                   \------>| Last prerelease : v1.1.0-beta |------->| Package2 2.1.0-beta |-------->| Package3 3.0.0 |-------->| Package4 4.0.0 |
        //                           +-------------------------------+        +---------------------+         +----------------+         +----------------+
        // 
        //                           +-----------------------+
        //                   /------>| Last release : v2.0.1 |
        // +----------+     /        +-----------------------+
        // | Package2 |-----
        // +----------+     \        +-------------------------------+         +----------------+         +----------------+
        //                   \------>| Last prerelease : v2.1.0-beta |-------->| Package3 3.0.0 |-------->| Package4 4.0.0 |
        //                           +-------------------------------+         +----------------+         +----------------+
        //
        // +----------+         +----------------+         +----------------+
        // | Package3 |-------->| Package3 3.1.0 |-------->| Package4 4.1.0 |
        // +----------+         +----------------+         +----------------+
        //
        // +----------+         +----------------+
        // | Package4 |-------->| Package4 4.1.0 |
        // +----------+         +----------------+
        [Test]
        public async Task Start_WithAnotherSimplePackageGraph_ShouldFillPackageRepositoryCorrectly()
        {
            IPackageDownloader fakeDownloader = Substitute.For<IPackageDownloader>();
            fakeDownloader.GetMaxVersion( new PackageId( PackageId.NuGet, "Package1" ) ).Returns( new PackageMaxVersion( "1.0.0", "1.1.0-beta" ) );
            fakeDownloader.GetMaxVersion( new PackageId( PackageId.NuGet, "Package2" ) ).Returns( new PackageMaxVersion( "2.0.1", "2.1.0-beta" ) );
            fakeDownloader.GetMaxVersion( new PackageId( PackageId.NuGet, "Package3" ) ).Returns( new PackageMaxVersion( "3.1.0" ) );
            fakeDownloader.GetMaxVersion( new PackageId( PackageId.NuGet, "Package4" ) ).Returns( new PackageMaxVersion( "4.1.0" ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package1", "1.0.0" ) )
                .Returns( new PackageInfo(
                    new VPackageId( PackageId.NuGet, "Package1", "1.0.0" ),
                    new Dictionary<PlatformId, IEnumerable<VPackageId>> { { PlatformId.None, new[] { new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) } } } ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package1", "1.1.0-beta" ) )
                .Returns( new PackageInfo(
                    new VPackageId( PackageId.NuGet, "Package1", "1.1.0-beta" ),
                    new Dictionary<PlatformId, IEnumerable<VPackageId>> { { PlatformId.None, new[] { new VPackageId( PackageId.NuGet, "Package2", "2.1.0-beta" ) } } } ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) )
                .Returns( new PackageInfo(new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package2", "2.0.1" ) )
                .Returns( new PackageInfo( new VPackageId( PackageId.NuGet, "Package2", "2.0.1" ) ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package2", "2.1.0-beta" ) )
                .Returns( new PackageInfo(
                    new VPackageId( PackageId.NuGet, "Package2", "2.1.0-beta" ),
                    new Dictionary<PlatformId, IEnumerable<VPackageId>> { { PlatformId.None, new[] { new VPackageId( PackageId.NuGet, "Package3", "3.0.0" ) } } } ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package3", "3.0.0" ) )
                .Returns( new PackageInfo(
                    new VPackageId( PackageId.NuGet, "Package3", "3.0.0" ),
                    new Dictionary<PlatformId, IEnumerable<VPackageId>> { { PlatformId.None, new[] { new VPackageId( PackageId.NuGet, "Package4", "4.0.0" ) } } } ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package3", "3.1.0" ) )
                .Returns( new PackageInfo(
                    new VPackageId( PackageId.NuGet, "Package3", "3.1.0" ),
                    new Dictionary<PlatformId, IEnumerable<VPackageId>> { { PlatformId.None, new[] { new VPackageId( PackageId.NuGet, "Package4", "4.1.0" ) } } } ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package4", "4.0.0" ) )
                .Returns( new PackageInfo( new VPackageId( PackageId.NuGet, "Package4", "4.0.0" ) ) );
            fakeDownloader.GetPackage( new VPackageId( PackageId.NuGet, "Package4", "4.1.0" ) )
                .Returns( new PackageInfo( new VPackageId( PackageId.NuGet, "Package4", "4.1.0" ) ) );

            FakePackageRepository fakeRepository = new FakePackageRepository();
            await fakeRepository.AddIfNotExists( new PackageId( PackageId.NuGet, "Package1" ) );

            ICrawler sut = CreateCrawler(
                fakeDownloader,
                fakeRepository,
                new PackageSegment( PackageId.NuGet, "a" ) );

            await sut.Start( 1 );

            Assert.That( await fakeRepository.GetAllPackages(), Is.EquivalentTo( new Package[]
            {
                new Package(
                    PackageId.NuGet,
                    "Package1",
                        new VPackage(
                            new VPackageId( PackageId.NuGet, "Package1", "1.0.0" ),
                            new[] { new VPackage( new VPackageId( PackageId.NuGet, "Package2", "2.0.0" ) ) } ),
                        new VPackage(
                            new VPackageId( PackageId.NuGet, "Package1", "1.1.0-beta" ),
                            new[]
                            {
                                new VPackage(
                                    new VPackageId( PackageId.NuGet, "Package2", "2.1.0-beta" ),
                                    new[]
                                    {
                                        new VPackage(
                                            new VPackageId( PackageId.NuGet, "Package3", "3.0.0" ),
                                            new[] { new VPackage( new VPackageId( PackageId.NuGet, "Package4", "4.0.0" ) ) } )
                                    } )
                            } ) ),
                new Package(
                    PackageId.NuGet,
                    "Package2",
                        new VPackage( new VPackageId( PackageId.NuGet, "Package2", "2.0.1" ) ),
                        new VPackage(
                            new VPackageId( PackageId.NuGet, "Package2", "2.1.0-beta" ),
                            new[]
                            {
                                new VPackage(
                                    new VPackageId( PackageId.NuGet, "Package3", "3.0.0" ),
                                    new[] { new VPackage( new VPackageId( PackageId.NuGet, "Package4", "4.0.0" ) ) } )
                            } ) ),
                new Package(
                    PackageId.NuGet,
                    "Package3",
                        new VPackage(
                            new VPackageId( PackageId.NuGet, "Package3", "3.1.0" ),
                            new[] { new VPackage(new VPackageId( PackageId.NuGet, "Package4", "4.1.0" ) ) } ) ),
                new Package(
                    PackageId.NuGet,
                    "Package4",
                        new VPackage(
                            new VPackageId( PackageId.NuGet, "Package4", "4.1.0" ) ) )
            } ) );
        }

        protected abstract ICrawler CreateCrawler(
            IPackageDownloader downloader,
            IPackageRepository repository,
            PackageSegment segment );
    }
}
