using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Abstractions.Tests;

namespace Invenietis.DependencyCrawler.IO.Tests
{
    public class NuGetDownloaderTests : PackageDownloaderTestsBase
    {
        protected override IPackageDownloader CreatePackageDownloader( IFeedProvider feedProvider )
        {
            return new NuGetDownloader( feedProvider );
        }
    }
}
