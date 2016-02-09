using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.IO.Tests
{
    [TestFixture]
    public class XmlPackageSerializerTests : PackageSerializerTestsBase
    {
        protected override IPackageSerializer CreateSerializer()
        {
            return new XmlPackageSerializer();
        }
    }
}
