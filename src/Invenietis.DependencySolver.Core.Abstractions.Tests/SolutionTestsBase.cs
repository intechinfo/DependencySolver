using System;
using NSubstitute;
using NUnit.Framework;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core.Abstractions.Tests
{
    [TestFixture]
    public abstract class SolutionTestsBase
    {
        [Test]
        public void CreateProject_WithValidInput_ShouldCreateANewProject()
        {
            string v = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution sut = repoVersion.CreateSolution( "TestSolution.sln" );

            IProject project1 = sut.CreateProject( "P1.csproj" );
            IProject project2 = sut.CreateProject( "P2.xproj" );

            Assert.That( sut.Projects, Is.EquivalentTo( new[] { project1, project2 } ) );
            Assert.That( project1.Solutions, Is.EquivalentTo( new[] { sut } ) );
            Assert.That( project2.Solutions, Is.EquivalentTo( new[] { sut } ) );
            Assert.That( project1.Path, Is.EqualTo( "P1.csproj" ) );
            Assert.That( project2.Path, Is.EqualTo( "P2.xproj" ) );
        }

        [Test]
        public void AddProject_WithAnExistingProject_ShouldAddTheProject()
        {
            string v = "v1.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution1.sln" );
            ISolution sut = repoVersion.CreateSolution( "TestSolution2.sln" );
            IProject project1 = solution.CreateProject( "P1.csproj" );
            IProject project2 = solution.CreateProject( "P2.csproj" );
            IProject project3 = sut.CreateProject( "P3.csproj" );

            sut.AddProject( project1 );

            Assert.That( sut.Projects, Is.EquivalentTo( new[] { project3, project1 } ) );
            Assert.That( project1.Solutions, Is.EquivalentTo( new[] { sut, solution } ) );
        }

        [Test]
        public void CreateProject_WithExistingProjectPath_ShouldThrowArgumentException()
        {
            string v = "v1.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution sut = repoVersion.CreateSolution( "TestSolution.sln" );
            string projectPath = "ProjectName.csproj";
            sut.CreateProject( projectPath );

            Assert.Throws<ArgumentException>( () => sut.CreateProject( projectPath ) );
        }

        [Test]
        public void AddProject_WithNullProject_ShouldThrowArgumentNullException()
        {
            string v = "v1.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution sut = repoVersion.CreateSolution( "TestSolution.sln" );

            Assert.Throws<ArgumentNullException>( () => sut.AddProject( null ) );
        }

        [Test]
        public void AddProject_WithProjectBelongsToAnotherContext_ShouldThrowArgumentNullException()
        {
            string v = "v1.0.0";
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepository repo1 = CreateGitRepository( @"C:\TestRepo1\" );
            IGitRepositoryVersion repoVersion1 = repo1.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion1.CreateSolution( "TestSolution1.sln" );
            IProject project = solution.CreateProject( "TestProject.csproj" );

            IGitRepository repo2 = CreateGitRepository( @"C:\TestRepo2\" );
            IGitRepositoryVersion repoVersion2 = repo2.CreateVersion( releaseTagVersion );
            ISolution sut = repoVersion2.CreateSolution( "TestSolution2.sln" );

            Assert.Throws<ArgumentException>( () => sut.AddProject( project ) );
        }

        [Test]
        public void AddProject_WithUnknownProject_ShouldThrowArgumentException()
        {
            string v = "v1.0.0";
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( v );
            IGitRepository repo1 = CreateGitRepository( @"C:\TestRepo1\" );
            IGitRepositoryVersion repoVersion1 = repo1.CreateVersion( releaseTagVersion );
            ISolution sut = repoVersion1.CreateSolution( "TestSolution1.sln" );
            IProject project = Substitute.For<IProject>();

            Assert.Throws<ArgumentException>( () => sut.AddProject( project ) );
        }

        protected abstract IGitRepository CreateGitRepository( string path );
    }
}
