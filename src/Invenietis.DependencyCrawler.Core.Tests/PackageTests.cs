using System;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Core.Tests
{
    [TestFixture]
    public class PackageInfoTests
    {
        [Test]
        public void Ctor_WithNullArg_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>( () => new PackageInfo( null, new VPackageId[ 0 ] ) );
            Assert.Throws<ArgumentNullException>( () => new PackageInfo( new VPackageId( "P", "1.0.0" ), null ) );
        }

        [Test]
        public void Ctor_WithValidInput_ShouldCreatePackageCorrectly()
        {
            PackageInfo sut = new PackageInfo(
                new VPackageId( "PackageId", "3.2.1" ),
                new[] { new VPackageId( "Package1", "1.0.0" ), new VPackageId( "Package2", "2.0.0" ) } );

            Assert.That( sut.VPackageInfo, Is.EqualTo( new VPackageId( "PackageId", "3.2.1" ) ) );
            Assert.That( sut.Dependencies, Is.EquivalentTo( new[] { new VPackageId( "Package1", "1.0.0" ), new VPackageId( "Package2", "2.0.0" ) } ) );
        }
    }
}
