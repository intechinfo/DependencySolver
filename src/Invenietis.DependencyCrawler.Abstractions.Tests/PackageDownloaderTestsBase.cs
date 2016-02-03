using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    [TestFixture]
    public abstract class PackageDownloaderTestsBase
    {
        // TestPackage1 versions :
        //  - 1.0.0
        //  - 1.0.0-b
        //
        // Git repo path : TestData\TestPackage1.git.zip
        //
        // So max version should be : 1.0.0
        [Test]
        public async Task GetMaxVersion_TestPackage1_ShouldReturnV1_0_0()
        {
            IFeedProvider fakeFeedProvider = Substitute.For<IFeedProvider>();
            fakeFeedProvider.Feeds.Returns( new[] { "https://www.myget.org/F/nugetdownloadertests/api/v2" } );
            IPackageDownloader sut = CreatePackageDownloader( fakeFeedProvider );

            Core.PackageMaxVersion maxVersion = await sut.GetMaxVersion( new Core.PackageId( "TestPackage1" ) );

            Assert.That( maxVersion.MaxVersion, Is.EqualTo( "1.0.0" ) );
            Assert.That( maxVersion.HasPreReleaseMaxVersion, Is.False );
        }

        // TestPackage1 versions :
        //  - 2.0.0-b
        //  - 1.0.1
        //  - 1.0.1-b
        //  - 1.0.0
        //
        // Git repo path : TestData\TestPackage2.git.zip
        //
        // So max version should be : 1.0.1 / 2.0.0-b
        [Test]
        public async Task GetMaxVersion_TestPackage2_ShouldReturnV1_0_1_V2_0_0_b()
        {
            IFeedProvider fakeFeedProvider = Substitute.For<IFeedProvider>();
            fakeFeedProvider.Feeds.Returns( new[] { "https://www.myget.org/F/nugetdownloadertests/api/v2" } );
            IPackageDownloader sut = CreatePackageDownloader( fakeFeedProvider );

            Core.PackageMaxVersion maxVersion = await sut.GetMaxVersion( new Core.PackageId( "TestPackage2" ) );

            Assert.That( maxVersion.MaxVersion, Is.EqualTo( "1.0.1" ) );
            Assert.That( maxVersion.PreReleaseMaxVersion, Is.EqualTo( "2.0.0-b" ) );
        }

        // TestPackage3 git repo path : TestData\TestPackage3.git.zip
        [Test]
        public async Task GetPackage_WithTestPackage3_ShouldReturnAPackageCorrectly()
        {
            IFeedProvider fakeFeedProvider = Substitute.For<IFeedProvider>();
            fakeFeedProvider.Feeds.Returns( new[] { "https://www.myget.org/F/nugetdownloadertests/api/v2" } );
            IPackageDownloader sut = CreatePackageDownloader( fakeFeedProvider );

            Core.PackageInfo package = await sut.GetPackage( new Core.VPackageId( "TestPackage3", "1.0.0" ) );

            Assert.That( package.VPackageInfo, Is.EqualTo( new Core.VPackageId( "TestPackage3", "1.0.0" ) ) );
            Assert.That( package.Dependencies, Is.EqualTo( new[]
            {
                new Core.VPackageId( "NUnit", "3.0.1" ),
                new Core.VPackageId( "EntityFramework.MicrosoftSqlServer", "7.0.0-rc1-final" ),
                new Core.VPackageId( "Microsoft.CSharp", "4.0.1-beta-23516" ),
                new Core.VPackageId( "System.Collections", "4.0.11-beta-23516" ),
                new Core.VPackageId( "System.Linq", "4.0.1-beta-23516" ),
                new Core.VPackageId( "System.Runtime", "4.0.21-beta-23516" ),
                new Core.VPackageId( "System.Threading", "4.0.11-beta-23516" )
            } ) );
        }

        protected abstract IPackageDownloader CreatePackageDownloader( IFeedProvider feedProvider );
    }
}
