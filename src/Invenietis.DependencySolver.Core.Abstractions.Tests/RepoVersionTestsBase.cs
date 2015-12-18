using System;
using NUnit.Framework;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core.Abstractions.Tests
{
    [TestFixture]
    public abstract class RepoVersionTestsBase
    {
        [Test]
        public void CreateSolution_WithValidSolutionName_ShouldCreateANewSolution()
        {
            string v = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepositoryVersion sut = repo.CreateVersion( releaseTagVersion );

            ISolution solution = sut.CreateSolution( "Solution.sln" );

            Assert.That( sut.Solutions, Is.EquivalentTo( new[] { solution } ) );
            Assert.That( solution.RepoVersion, Is.SameAs( sut ) );
            Assert.That( solution.Path, Is.EqualTo( "Solution.sln" ) );
        }

        [Test]
        public void CreateSolution_WithNullOrEmptySolutionName_ShouldThrowAnArgumentException()
        {
            string v = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepositoryVersion sut = repo.CreateVersion( releaseTagVersion );

            Assert.Throws<ArgumentException>( () => sut.CreateSolution( string.Empty ) );
            Assert.Throws<ArgumentException>( () => sut.CreateSolution( null ) );
            Assert.Throws<ArgumentException>( () => sut.CreateSolution( "  " ) );
        }

        [Test]
        public void CreateSolution_WithExistingSolutionName_ShouldThrowAnArgumentException()
        {
            string v = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepositoryVersion sut = repo.CreateVersion( releaseTagVersion );
            sut.CreateSolution( "SolutionName.sln" );

            Assert.Throws<ArgumentException>( () => sut.CreateSolution( "SolutionName.sln" ) );
        }

        protected abstract IGitRepository CreateGitRepository( string path );
    }
}
