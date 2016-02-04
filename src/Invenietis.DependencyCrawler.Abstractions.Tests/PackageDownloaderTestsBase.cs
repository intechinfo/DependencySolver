using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;
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

            PackageMaxVersion maxVersion = await sut.GetMaxVersion( new PackageId( PackageId.NuGet, "TestPackage1" ) );

            Assert.That( maxVersion.ReleaseMaxVersion, Is.EqualTo( "1.0.0" ) );
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

            PackageMaxVersion maxVersion = await sut.GetMaxVersion( new PackageId( PackageId.NuGet, "TestPackage2" ) );

            Assert.That( maxVersion.ReleaseMaxVersion, Is.EqualTo( "1.0.1" ) );
            Assert.That( maxVersion.PreReleaseMaxVersion, Is.EqualTo( "2.0.0-b" ) );
        }

        // TestPackage3 git repo path : TestData\TestPackage3.git.zip
        [Test]
        public async Task GetPackage_WithTestPackage3_ShouldReturnAPackageCorrectly()
        {
            IFeedProvider fakeFeedProvider = Substitute.For<IFeedProvider>();
            fakeFeedProvider.Feeds.Returns( new[] { "https://www.myget.org/F/nugetdownloadertests/api/v2" } );
            IPackageDownloader sut = CreatePackageDownloader( fakeFeedProvider );

            PackageInfo package = await sut.GetPackage( new VPackageId( PackageId.NuGet, "TestPackage3", "1.0.0" ) );

            Assert.That( package.VPackageId, Is.EqualTo( new VPackageId( PackageId.NuGet, "TestPackage3", "1.0.0" ) ) );
            Assert.That( package.Dependencies, Is.EqualTo( new[]
            {
                new VPackageId( PackageId.NuGet, "NUnit", "3.0.1" ),
                new VPackageId( PackageId.NuGet, "EntityFramework.MicrosoftSqlServer", "7.0.0-rc1-final" ),
                new VPackageId( PackageId.NuGet, "Microsoft.CSharp", "4.0.1-beta-23516" ),
                new VPackageId( PackageId.NuGet, "System.Collections", "4.0.11-beta-23516" ),
                new VPackageId( PackageId.NuGet, "System.Linq", "4.0.1-beta-23516" ),
                new VPackageId( PackageId.NuGet, "System.Runtime", "4.0.21-beta-23516" ),
                new VPackageId( PackageId.NuGet, "System.Threading", "4.0.11-beta-23516" )
            } ) );
        }

        protected abstract IPackageDownloader CreatePackageDownloader( IFeedProvider feedProvider );
    }
}
