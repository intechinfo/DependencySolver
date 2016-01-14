using System;
using NSubstitute;
using NUnit.Framework;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core.Abstractions.Tests
{
    [TestFixture]
    public abstract class ProjectTestsBase
    {
        [Test]
        public void CreateDependency_WithValidInputs_ShouldCreateANewDependency()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );

            IProjectDependency dependency1 = sut.CreateDependency( "Package1", "1.2.3" );
            IProjectDependency dependency2 = sut.CreateDependency( "Package2", "3.2.1" );

            Assert.That( sut.Dependencies, Is.EquivalentTo( new[] { dependency1, dependency2 } ) );
            Assert.That( dependency1.Projects, Is.EquivalentTo( new[] { sut } ) );
            Assert.That( dependency2.Projects, Is.EquivalentTo( new[] { sut } ) );
            Assert.That( dependency1.Name, Is.EqualTo( "Package1" ) );
            Assert.That( dependency2.Name, Is.EqualTo( "Package2" ) );
            Assert.That( dependency1.Version, Is.EqualTo( "1.2.3" ) );
            Assert.That( dependency2.Version, Is.EqualTo( "3.2.1" ) );
        }

        [Test]
        public void AddDependency_WithAnExistingDependency_ShouldAddThisDependency()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );
            IProject project = solution.CreateProject( "P2.csproj" );
            IProjectDependency dependency1 = sut.CreateDependency( "Package1", "1.2.3" );
            IProjectDependency dependency2 = sut.CreateDependency( "Package2", "3.2.1" );
            IProjectDependency dependency3 = project.CreateDependency( "Package3", "2.0.0" );
            IProjectDependency dependency4 = project.CreateDependency( "Package4", "1.0.0" );

            sut.AddDependency( dependency3 );

            Assert.That( sut.Dependencies, Is.EquivalentTo( new[] { dependency1, dependency2, dependency3 } ) );
            Assert.That( dependency3.Projects, Is.EquivalentTo( new[] { project, sut } ) );
        }

        [Test]
        public void CreateDependency_WithInvalidInputs_ShouldThrowArgumentException()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );

            Assert.Throws<ArgumentException>( () => sut.CreateDependency( string.Empty, "1.2.3" ) );
            Assert.Throws<ArgumentException>( () => sut.CreateDependency( null, "1.2.3" ) );
            Assert.Throws<ArgumentException>( () => sut.CreateDependency( " ", "1.2.3" ) );
            Assert.Throws<ArgumentException>( () => sut.CreateDependency( "TestPackage", string.Empty ) );
            Assert.Throws<ArgumentException>( () => sut.CreateDependency( "TestPackage", null ) );
            Assert.Throws<ArgumentException>( () => sut.CreateDependency( "TestPackage", " " ) );
        }

        [Test]
        public void AddDependency_WithDependencyBelongsToAnotherContext_ShouldThrowArgumentException()
        {
            string solutionVersion = "v0.0.0";
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepository repo1 = CreateGitRepository( @"C:\TestRepo\" );
            IGitRepositoryVersion repoVersion1 = repo1.CreateVersion( releaseTagVersion );
            ISolution solution1 = repoVersion1.CreateSolution( "TestSolution.sln" );
            IProject project = solution1.CreateProject( "P1.csproj" );
            IProjectDependency dependency = project.CreateDependency( "TestPackage", "1.0.0" );

            IGitRepository repo2 = CreateGitRepository( @"C:\TestRepo\" );
            IGitRepositoryVersion repoVersion2 = repo2.CreateVersion( releaseTagVersion );
            ISolution solution2 = repoVersion2.CreateSolution( "TestSolution.sln" );
            IProject sut = solution2.CreateProject( "P1.csproj" );

            Assert.Throws<ArgumentException>( () => sut.AddDependency( dependency ) );
        }

        [Test]
        public void AddDependency_WithNullDependency_ShouldThrowArgumentNullException()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );

            Assert.Throws<ArgumentNullException>( () => sut.AddDependency( null ) );
        }

        [Test]
        public void AddDependency_WithUnknownDependency_ShouldThrowArgumentNullException()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );
            IProjectDependency dependency = Substitute.For<IProjectDependency>();

            Assert.Throws<ArgumentException>( () => sut.AddDependency( dependency ) );
        }

        [Test]
        public void AddSolution_WithValidSolution_ShouldAddTheSolution()
        {
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( "v0.0.0" );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution1 = repoVersion.CreateSolution( "TestSolution1.sln" );
            ISolution solution2 = repoVersion.CreateSolution( "TestSolution2.sln" );
            IProject sut = solution1.CreateProject( @"P1\P1.xproj" );

            sut.AddSolution( solution2 );

            Assert.That( sut.Solutions, Is.EquivalentTo( new[] { solution1, solution2 } ) );
            Assert.That( solution1.Projects, Is.EquivalentTo( new[] { sut } ) );
            Assert.That( solution2.Projects, Is.EquivalentTo( new[] { sut } ) );
        }

        protected abstract IGitRepository CreateGitRepository( string path );
    }
}
