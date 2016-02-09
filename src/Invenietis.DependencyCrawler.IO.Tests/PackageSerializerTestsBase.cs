using System.Collections.Generic;
using System.Linq;
using Invenietis.DependencyCrawler.Core;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.IO.Tests
{
    [TestFixture]
    public abstract class PackageSerializerTestsBase
    {
        [Test]
        public void SerializeThenDeserialize_ShouldReturnOriginalData()
        {
            IPackageSerializer sut = CreateSerializer();
            VPackage vPackage = new VPackage(
                new VPackageId( "NuGet", "CK.StObj.Engine", "4.0.0" ),
                new[]
                {
                    new Platform(
                        new PlatformId("DNXCore5.0"),
                        new[]
                        {
                            new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.0" ) ),
                            new VPackage( new VPackageId( "NuGet", "CK.Reflection", "4.3.0" ) ),
                            new VPackage(
                                new VPackageId( "NuGet", "CK.Setup.Dependency", "4.0.0" ),
                                new[]
                                {
                                    new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.0" ) )
                                } ),
                            new VPackage(
                                new VPackageId( "NuGet", "CK.StObj.Model", "4.0.0" ),
                                new[]
                                {
                                    new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.0" ) )
                                } ),
                            new VPackage(
                                new VPackageId( "NuGet", "CK.StObj.Runtime", "4.0.0" ),
                                new[]
                                {
                                    new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.0" ) ),
                                    new VPackage(
                                        new VPackageId( "NuGet", "CK.StObj.Model", "4.0.0" ),
                                        new[]
                                        {
                                            new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.0" ) )
                                        } )
                                } )
                        }),
                    new Platform(
                        new PlatformId(".NETPlatform5.0"),
                        new[]
                        {
                            new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.1" ) ),
                            new VPackage( new VPackageId( "NuGet", "CK.Reflection", "4.3.0" ) ),
                            new VPackage(
                                new VPackageId( "NuGet", "CK.Setup.Dependency", "4.1.0" ),
                                new[]
                                {
                                    new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.1" ) )
                                } ),
                            new VPackage(
                                new VPackageId( "NuGet", "CK.StObj.Model", "4.1.0" ),
                                new[]
                                {
                                    new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.1" ) )
                                } ),
                            new VPackage(
                                new VPackageId( "NuGet", "CK.StObj.Runtime", "4.1.0" ),
                                new[]
                                {
                                    new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.1" ) ),
                                    new VPackage(
                                        new VPackageId( "NuGet", "CK.StObj.Model", "4.1.0" ),
                                        new[]
                                        {
                                            new VPackage( new VPackageId( "NuGet", "CK.Core", "4.3.1" ) )
                                        } )
                                } )
                        })
                } );

            string serializedVPackage = sut.Serialize( vPackage );
            VPackage result = sut.DeserializeVPackage( serializedVPackage );

            Assert.That( result, Is.EqualTo( vPackage ) );
        }

        protected abstract IPackageSerializer CreateSerializer();
    }
}
