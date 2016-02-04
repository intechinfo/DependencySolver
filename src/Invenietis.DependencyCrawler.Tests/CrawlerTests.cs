using Invenietis.DependencyCrawler.Abstractions.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Tests
{
    [TestFixture]
    public class CrawlerTests : CrawlerTestsBase
    {
        protected override ICrawler CreateCrawler(
            IPackageDownloader downloader,
            IPackageRepository repository,
            PackageSegment segment )
        {
            return new Crawler(
                downloader,
                repository,
                segment );
        }
    }
}
