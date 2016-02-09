using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Core.Tests
{
    [TestFixture]
    public class PackageInfoTests
    {
        [Test]
        public void Ctor_WithNullArg_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>( () => new PackageInfo( null, new Dictionary<PlatformId, IEnumerable<VPackageId>>() ) );
        }

        [Test]
        public void Ctor_WithValidInput_ShouldCreatePackageCorrectly()
        {
            PackageInfo sut = new PackageInfo(
                new VPackageId( PackageId.NuGet, "PackageId", "3.2.1" ),
                new Dictionary<PlatformId, IEnumerable<VPackageId>>
                {
                    {
                        PlatformId.None,
                        new[]
                        {
                            new VPackageId( PackageId.NuGet, "Package1", "1.0.0" ),
                            new VPackageId( PackageId.NuGet, "Package2", "2.0.0" )
                        }
                    }
                } );

            Assert.That( sut.VPackageId, Is.EqualTo( new VPackageId( PackageId.NuGet, "PackageId", "3.2.1" ) ) );
            Assert.That( sut.Dependencies, Is.EquivalentTo(
                new Dictionary<PlatformId, IEnumerable<VPackageId>>
                {
                    {
                        PlatformId.None,
                        new[]
                        {
                            new VPackageId( PackageId.NuGet, "Package1", "1.0.0" ),
                            new VPackageId( PackageId.NuGet, "Package2", "2.0.0" )
                        }
                    }
                } ) );
        }
    }
}
