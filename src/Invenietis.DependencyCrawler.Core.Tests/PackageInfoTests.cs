using System;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Core.Tests
{
    [TestFixture]
    public class VPackageInfoTests
    {
        [Test]
        public void Ctor_WithInvalidArg_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentException>( () => new VPackageId( null, "1.0.0" ) );
            Assert.Throws<ArgumentException>( () => new VPackageId( "", "1.0.0" ) );
            Assert.Throws<ArgumentException>( () => new VPackageId( "  ", "1.0.0" ) );
            Assert.Throws<ArgumentException>( () => new VPackageId( "PackageId", null ) );
            Assert.Throws<ArgumentException>( () => new VPackageId( "PackageId", "" ) );
            Assert.Throws<ArgumentException>( () => new VPackageId( "PackageId", "   " ) );
        }

        [Test]
        public void Ctor_WithValidInput_ShouldCreatePackageInfoCorrectly()
        {
            VPackageId sut = new VPackageId( "PackageId", "1.0.0" );
            Assert.That( sut.Id, Is.EqualTo( "PackageId" ) );
            Assert.That( sut.Version, Is.EqualTo( "1.0.0" ) );
        }

        [TestCase( "PackageId", "1.0.0", true )]
        [TestCase( "WrongPackageId", "1.0.0", false )]
        [TestCase( "PackageId", "2.0.0", false )]
        public void Equals_WithSomePackages( string packageId, string version, bool expected )
        {
            VPackageId sut = new VPackageId( "PackageId", "1.0.0" );
            VPackageId other = new VPackageId( packageId, version );

            Assert.That( sut.Equals( other ), Is.EqualTo( expected ) );
        }


        [Test]
        public void Equals_WithNonPackageInfo_ShouldReturnFalse()
        {
            VPackageId sut = new VPackageId( "PackageId", "1.0.0" );
            Assert.That( sut.Equals( null ), Is.False );
            Assert.That( sut.Equals( "wrong type" ), Is.False );
        }
    }
}
