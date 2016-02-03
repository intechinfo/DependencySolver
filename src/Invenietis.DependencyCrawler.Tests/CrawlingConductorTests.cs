using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Abstractions.Tests;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Tests
{
    [TestFixture]
    public class CrawlingConductorTests : CrawlingConductorTestsBase
    {
        protected override ICrawlingConductor CreateCrawlingConductor( IInJobQueue queue, IPackageRepository packageRepository )
        {
            return new CrawlingConductor( queue, packageRepository );
        }
    }
}
