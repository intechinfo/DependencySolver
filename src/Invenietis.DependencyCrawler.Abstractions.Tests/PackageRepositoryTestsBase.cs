using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    [TestFixture]
    public abstract class PackageRepositoryTestsBase
    {
        [Test]
        public async Task ExistsVPackage_WithAnExistingVPackage_ShouldReturnTrue()
        {
            IPackageRepository sut = CreatePackageRepository();
            string packageName = $"Test{Guid.NewGuid()}";
            VPackageId vPackageId = new VPackageId( packageName, "1.0.0" );
            await sut.AddOrUpdateVPackage( new VPackage( vPackageId ) );

            bool result = await sut.ExistsVPackage( vPackageId );

            Assert.That( result, Is.True );
        }

        [Test]
        public async Task ExistsVPackage_WithANotExistingVPackage_ShouldReturnFalse()
        {
            IPackageRepository sut = CreatePackageRepository();
            string packageName = $"Test{Guid.NewGuid()}";
            VPackageId vPackageId = new VPackageId( packageName, "1.0.0" );

            bool result = await sut.ExistsVPackage( vPackageId );

            Assert.That( result, Is.False );
        }

        [Test]
        public async Task GetVPackageById_WithAnExistingVPackage_ShouldReturnThisVPackage()
        {
            IPackageRepository sut = CreatePackageRepository();
            string packageName = $"Test{Guid.NewGuid()}";
            VPackageId vPackageId = new VPackageId( packageName, "1.0.0" );
            VPackage vPackage = new VPackage( vPackageId );
            await sut.AddOrUpdateVPackage( vPackage );

            VPackage result = await sut.GetVPackageById( vPackageId );

            Assert.That( result, Is.EqualTo( vPackage ) );
        }

        [Test]
        public async Task GetVPackageById_WithANotExistingVPackage_ShouldReturnNull()
        {
            IPackageRepository sut = CreatePackageRepository();
            string packageName = $"Test{Guid.NewGuid()}";
            VPackageId vPackageId = new VPackageId( packageName, "1.0.0" );
            VPackage vPackage = new VPackage( vPackageId );

            VPackage result = await sut.GetVPackageById( vPackageId );

            Assert.That( result, Is.Null );
        }

        [Test]
        public async Task GetVPackageById_WithVPackageContainingDependencies_ShouldReturnThisVPackage()
        {
            IPackageRepository sut = CreatePackageRepository();
            string packageName = $"Test{Guid.NewGuid()}";
            VPackageId vPackageId = new VPackageId( packageName, "1.0.0" );
            VPackage vPackage = new VPackage( vPackageId, new[]
            {
                new VPackageId( "P1", "1.2.3" ),
                new VPackageId( "P2", "2.3.4" )
            } );
            await sut.AddOrUpdateVPackage( vPackage );

            VPackage result = await sut.GetVPackageById( vPackageId );

            Assert.That( result, Is.EqualTo( vPackage ) );
        }

        [Test]
        public async Task GetVPackageById_WithNotFoundVPackage_ShouldReturnThisVPackage()
        {
            IPackageRepository sut = CreatePackageRepository();
            string packageName = $"Test{Guid.NewGuid()}";
            VPackageId vPackageId = new VPackageId( packageName, "1.0.0" );
            VPackage vPackage = new VPackage( vPackageId, true );
            await sut.AddOrUpdateVPackage( vPackage );

            VPackage result = await sut.GetVPackageById( vPackageId );

            Assert.That( result, Is.EqualTo( vPackage ) );
        }

        [Test]
        public async Task GetPackageById_WithNotExistingPackage_ShouldReturnNull()
        {
            IPackageRepository sut = CreatePackageRepository();
            PackageId packageId = new PackageId( $"Test{Guid.NewGuid()}" );

            Package result = await sut.GetPackageById( packageId );

            Assert.That( result, Is.Null );
        }

        [Test]
        public async Task GetPackageById_WithExistingPackage_ShouldReturnThisPackage()
        {
            IPackageRepository sut = CreatePackageRepository();
            string packageName = $"Test{Guid.NewGuid()}";
            PackageId packageId = new PackageId( packageName );
            Package package = new Package(
                packageName,
                new VPackage(
                    new VPackageId( "P1", "1.0.0" ),
                    new[] { new VPackageId( "P2", "2.0.0" ) } ),
                new VPackage(
                    new VPackageId( "P1", "1.0.1-beta" ),
                    new[] { new VPackageId( "P2", "2.0.0" ) } ) );
            await sut.AddOrUpdatePackage( package );

            Package result = await sut.GetPackageById( packageId );

            Assert.That( result, Is.EqualTo( package ) );
        }

        [Test]
        public async Task AddOrUpdateVPackage_WithExistingVPackage_ShouldUpdateTheVPackage()
        {
            IPackageRepository sut = CreatePackageRepository();

            string packageName = $"Test{Guid.NewGuid()}";
            VPackageId vPackageId = new VPackageId( packageName, "1.0.0" );
            VPackage vPackage1 = new VPackage( vPackageId );
            await sut.AddOrUpdateVPackage( vPackage1 );

            VPackage vPackage2 = new VPackage( vPackageId, new[] { new VPackageId( "P1", "1.0.0" ) } );
            await sut.AddOrUpdateVPackage( vPackage2 );

            VPackage result = await sut.GetVPackageById( vPackageId );

            Assert.That( result, Is.EqualTo( vPackage2 ) );
        }

        [Test]
        public async Task AddOrUpdatePackage_WithExistingPackage_ShouldUpdateThePackage()
        {
            IPackageRepository sut = CreatePackageRepository();
            string packageName = $"Test{Guid.NewGuid()}";
            PackageId packageId = new PackageId( packageName );

            {
                Package tmpPackage = new Package(
                    packageName,
                    new VPackage(
                        new VPackageId( "P1", "1.0.0" ),
                        new[] { new VPackageId( "P2", "2.0.0" ) } ),
                    new VPackage(
                        new VPackageId( "P1", "1.0.1-beta" ),
                        new[] { new VPackageId( "P2", "2.0.0" ) } ) );
                await sut.AddOrUpdatePackage( tmpPackage );
            }

            Package package;
            {
                package = new Package(
                    packageName,
                    new VPackage(
                        new VPackageId( "P1", "1.0.1" ),
                        new[] { new VPackageId( "P2", "2.0.0" ) } ),
                    new VPackage(
                        new VPackageId( "P1", "2.0.0-beta" ),
                        new[] { new VPackageId( "P2", "3.0.0" ) } ) );
                await sut.AddOrUpdatePackage( package );
            }

            Package result = await sut.GetPackageById( packageId );

            Assert.That( result, Is.EqualTo( package ) );
        }

        protected abstract IPackageRepository CreatePackageRepository();
    }
}
