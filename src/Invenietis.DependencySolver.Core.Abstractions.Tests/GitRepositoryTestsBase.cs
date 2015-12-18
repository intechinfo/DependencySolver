using System;
using NUnit.Framework;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core.Abstractions.Tests
{
    [TestFixture]
    public abstract class GitRepositoryTestsBase
    {
        [Test]
        public void CreateNew_ShouldReturnAnEmptyRepo()
        {
            IGitRepository sut = CreateGitRepository( @"C:\TestRepo\" );

            Assert.That( sut.RepoVersions.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void CreateVersion_WithValidReleaseTag_ShouldCreateANewVersion()
        {
            string v = "v0.0.0";
            IGitRepository sut = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );

            IGitRepositoryVersion version = sut.CreateVersion( releaseTagVersion );

            Assert.That( sut.RepoVersions, Is.EquivalentTo( new[] { version } ) );
            Assert.That( version.ReleaseTagVersion, Is.EqualTo( ReleaseTagVersion.TryParse( v ) ) );
            Assert.That( version.GitRepository, Is.SameAs( sut ) );
        }

        [Test]
        public void CreateVersion_WithInvalidReleaseTag_ShouldThrowAnArgumentException()
        {
            string v = "InvalidReleaseTag";
            IGitRepository sut = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );

            Assert.Throws<ArgumentException>( () => sut.CreateVersion( releaseTagVersion ) );
        }

        [Test]
        public void CreateVersion_WithExistingVersion_ShouldThrowAnArgumentException()
        {
            string v = "v0.0.0";
            IGitRepository sut = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepositoryVersion version = sut.CreateVersion( releaseTagVersion );

            Assert.Throws<ArgumentException>( () => sut.CreateVersion( releaseTagVersion ) );
        }

        protected abstract IGitRepository CreateGitRepository( string path );
    }
}
