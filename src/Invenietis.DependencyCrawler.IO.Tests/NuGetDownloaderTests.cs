using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Abstractions.Tests;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.IO.Tests
{
    [TestFixture]
    public class NuGetDownloaderTests : PackageDownloaderTestsBase
    {
        protected override IPackageDownloader CreatePackageDownloader( IFeedProvider feedProvider )
        {
            return new NuGetDownloader( feedProvider );
        }
    }
}
