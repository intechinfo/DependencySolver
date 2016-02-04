﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;
using System.Linq;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    [TestFixture]
    public abstract class PackageRepositoryTestsBase
    {
        [Test]
        public async Task AddIfNotExists_WithNewPackageId_ShouldAddThisPackageId()
        {
            IPackageRepository sut = CreatePackageRepository();
            PackageId packageId = new PackageId( "Test", Guid.NewGuid().ToString() );

            await sut.AddIfNotExists( packageId );

            IEnumerable<PackageId> packageIds = await sut.GetPackageIds( new PackageSegment( "Test", packageId.Value, $"{packageId.Value}a" ) );
            Assert.That( packageIds, Contains.Item( packageId ) );
        }

        [Test]
        public async Task AddIfNotExists_WithNewPackageId_ShouldReturnTrue()
        {
            IPackageRepository sut = CreatePackageRepository();
            PackageId packageId = new PackageId( "Test", Guid.NewGuid().ToString() );

            bool result = await sut.AddIfNotExists( packageId );

            Assert.That( result, Is.True );
        }

        [Test]
        public async Task AddIfNotExists_WithAnExistingPackageId_ShouldReturnFalse()
        {
            IPackageRepository sut = CreatePackageRepository();
            PackageId packageId = new PackageId( "Test", Guid.NewGuid().ToString() );
            bool result = await sut.AddIfNotExists( packageId );

            result = await sut.AddIfNotExists( packageId );

            Assert.That( result, Is.False );
        }

        [Test]
        public async Task AddIfNotExists_WithNewVPackageId_ShouldAddThisVPackageId()
        {
            IPackageRepository sut = CreatePackageRepository();
            VPackageId vPackageId = new VPackageId( "Test", Guid.NewGuid().ToString(), "1.0.0" );

            await sut.AddIfNotExists( vPackageId );

            IEnumerable<VPackageId> vPackageIds = await sut.GetVPackageIds( new PackageSegment( "Test", vPackageId.Id ) );
            Assert.That( vPackageIds, Contains.Item( vPackageId ) );
        }

        [Test]
        public async Task AddIfNotExists_WithNewVPackageId_ShouldReturnTrue()
        {
            IPackageRepository sut = CreatePackageRepository();
            VPackageId vPackageId = new VPackageId( "Test", Guid.NewGuid().ToString(), "1.0.0" );

            bool result = await sut.AddIfNotExists( vPackageId );

            Assert.That( result, Is.True );
        }

        [Test]
        public async Task AddIfNotExists_WithAnExistingVPackageId_ShouldReturnFalse()
        {
            IPackageRepository sut = CreatePackageRepository();
            VPackageId vPackageId = new VPackageId( "Test", Guid.NewGuid().ToString(), "1.0.0" );
            await sut.AddIfNotExists( vPackageId );

            bool result = await sut.AddIfNotExists( vPackageId );

            Assert.That( result, Is.False );
        }

        [Test]
        public async Task UpdateLastRelease_WithValidInput_ShouldUpdateLastRelease()
        {
            IPackageRepository sut = CreatePackageRepository();
            PackageId packageId = new PackageId( "Test", Guid.NewGuid().ToString() );
            await sut.AddIfNotExists( packageId );
            VPackageId lastRelease = new VPackageId( "Test", packageId.Value, "1.0.0" );
            await sut.AddIfNotExists( lastRelease );

            await sut.UpdateLastRelease( packageId, lastRelease );

            Package package = await sut.GetPackageById( packageId );
            Assert.That( package, Is.EqualTo(
                new Package(
                    packageId,
                    new VPackage( lastRelease ) ) ) );
        }

        [Test]
        public async Task UpdateLastPreRelease_WithValidInput_ShouldUpdateLastPreRelease()
        {
            IPackageRepository sut = CreatePackageRepository();
            PackageId packageId = new PackageId( "Test", Guid.NewGuid().ToString() );
            await sut.AddIfNotExists( packageId );
            VPackageId lastPreRelease = new VPackageId( "Test", packageId.Value, "2.0.0-alpha" );
            await sut.AddIfNotExists( lastPreRelease );

            await sut.UpdateLastPreRelease( packageId, lastPreRelease );

            Package package = await sut.GetPackageById( packageId );
            Assert.That( package, Is.EqualTo(
                new Package(
                    packageId,
                    null,
                    new VPackage( lastPreRelease ) ) ) );
        }

        [Test]
        public async Task AddDependenciesIfNotExists_With2Depedencies_ShouldAddThis2Dependencies()
        {
            IPackageRepository sut = CreatePackageRepository();
            PackageId packageId = new PackageId( "Test", Guid.NewGuid().ToString() );
            await sut.AddIfNotExists( packageId );
            VPackageId lastRelease = new VPackageId( "Test", packageId.Value, "1.0.0" );
            VPackageId lastPreRelease = new VPackageId( "Test", packageId.Value, "2.0.0-alpha" );
            await sut.AddIfNotExists( lastRelease );
            await sut.AddIfNotExists( lastPreRelease );
            await sut.UpdateLastRelease( packageId, lastRelease );
            await sut.UpdateLastPreRelease( packageId, lastPreRelease );
            VPackageId dependencyId1 = new VPackageId( "Test", Guid.NewGuid().ToString(), "1.0.0" );
            VPackageId dependencyId2 = new VPackageId( "Test", Guid.NewGuid().ToString(), "1.0.0" );
            IEnumerable<VPackageId> dependencyIds = new[] { dependencyId1, dependencyId2 };

            await sut.AddDependenciesIfNotExists( lastRelease, dependencyIds );

            Package package = await sut.GetPackageById( packageId );
            Assert.That( package, Is.EqualTo(
                new Package(
                    packageId,
                    new VPackage(
                        lastRelease,
                        new[]
                        {
                            new VPackage( dependencyId1 ),
                            new VPackage( dependencyId2 )
                        } ),
                    new VPackage( lastPreRelease ) ) ) );
        }

        [Test]
        public async Task GetNotCrawledVPackageIds_WorksCorrectly()
        {
            IPackageRepository sut = CreatePackageRepository();
            VPackageId vPackageId = new VPackageId( "Test", Guid.NewGuid().ToString(), "1.0.0" );
            await sut.AddIfNotExists( vPackageId );

            IEnumerable<VPackageId> notCrawled = await sut.GetNotCrawledVPackageIds( new PackageSegment( "Test", vPackageId.Id ) );
            Assert.That( notCrawled, Contains.Item( vPackageId ) );

            await sut.AddDependenciesIfNotExists( vPackageId, null );

            notCrawled = await sut.GetNotCrawledVPackageIds( new PackageSegment( "Test", vPackageId.Id ) );
            Assert.That( notCrawled.Count( x => x == vPackageId ), Is.EqualTo( 0 ) );
        }

        protected abstract IPackageRepository CreatePackageRepository();
    }
}
