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
        public void CreateNugetPackage_WithValidInputs_ShouldCreateANewNugetPackage()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );

            IProjectDependency package1 = sut.CreateNugetPackage( "Package1", "1.2.3" );
            IProjectDependency package2 = sut.CreateNugetPackage( "Package2", "3.2.1" );

            Assert.That( sut.Packages, Is.EquivalentTo( new[] { package1, package2 } ) );
            Assert.That( package1.Projects, Is.EquivalentTo( new[] { sut } ) );
            Assert.That( package2.Projects, Is.EquivalentTo( new[] { sut } ) );
            Assert.That( package1.Name, Is.EqualTo( "Package1" ) );
            Assert.That( package2.Name, Is.EqualTo( "Package2" ) );
            Assert.That( package1.Version, Is.EqualTo( "1.2.3" ) );
            Assert.That( package2.Version, Is.EqualTo( "3.2.1" ) );
        }

        [Test]
        public void AddNugetPackage_WithAnExistingPackage_ShouldAddThisPackage()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );
            IProject project = solution.CreateProject( "P2.csproj" );
            IProjectDependency package1 = sut.CreateNugetPackage( "Package1", "1.2.3" );
            IProjectDependency package2 = sut.CreateNugetPackage( "Package2", "3.2.1" );
            IProjectDependency package3 = project.CreateNugetPackage( "Package3", "2.0.0" );
            IProjectDependency package4 = project.CreateNugetPackage( "Package4", "1.0.0" );

            sut.AddNugetPackage( package3 );

            Assert.That( sut.Packages, Is.EquivalentTo( new[] { package1, package2, package3 } ) );
            Assert.That( package3.Projects, Is.EquivalentTo( new[] { project, sut } ) );
        }

        [Test]
        public void CreateNugetPackage_WithInvalidInputs_ShouldThrowArgumentException()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );

            Assert.Throws<ArgumentException>( () => sut.CreateNugetPackage( string.Empty, "1.2.3" ) );
            Assert.Throws<ArgumentException>( () => sut.CreateNugetPackage( null, "1.2.3" ) );
            Assert.Throws<ArgumentException>( () => sut.CreateNugetPackage( " ", "1.2.3" ) );
            Assert.Throws<ArgumentException>( () => sut.CreateNugetPackage( "TestPackage", string.Empty ) );
            Assert.Throws<ArgumentException>( () => sut.CreateNugetPackage( "TestPackage", null ) );
            Assert.Throws<ArgumentException>( () => sut.CreateNugetPackage( "TestPackage", " " ) );
        }

        [Test]
        public void AddNugetPackage_WithPackageBelongsToAnotherContext_ShouldThrowArgumentException()
        {
            string solutionVersion = "v0.0.0";
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepository repo1 = CreateGitRepository( @"C:\TestRepo\" );
            IGitRepositoryVersion repoVersion1 = repo1.CreateVersion( releaseTagVersion );
            ISolution solution1 = repoVersion1.CreateSolution( "TestSolution.sln" );
            IProject project = solution1.CreateProject( "P1.csproj" );
            IProjectDependency package = project.CreateNugetPackage( "TestPackage", "1.0.0" );

            IGitRepository repo2 = CreateGitRepository( @"C:\TestRepo\" );
            IGitRepositoryVersion repoVersion2 = repo2.CreateVersion( releaseTagVersion );
            ISolution solution2 = repoVersion2.CreateSolution( "TestSolution.sln" );
            IProject sut = solution2.CreateProject( "P1.csproj" );

            Assert.Throws<ArgumentException>( () => sut.AddNugetPackage( package ) );
        }

        [Test]
        public void AddNugetPackage_WithNullPackage_ShouldThrowArgumentNullException()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );

            Assert.Throws<ArgumentNullException>( () => sut.AddNugetPackage( null ) );
        }

        [Test]
        public void AddNugetPackage_WithUnknownPackage_ShouldThrowArgumentNullException()
        {
            string solutionVersion = "v0.0.0";
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( solutionVersion );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution.sln" );
            IProject sut = solution.CreateProject( "P1.csproj" );
            IProjectDependency package = Substitute.For<IProjectDependency>();

            Assert.Throws<ArgumentException>( () => sut.AddNugetPackage( package ) );
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
