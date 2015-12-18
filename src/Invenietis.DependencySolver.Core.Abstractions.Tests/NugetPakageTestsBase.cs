using NUnit.Framework;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core.Abstractions.Tests
{
    [TestFixture]
    public abstract class NugetPakageTestsBase
    {
        [Test]
        public void AddProject_WithValidProject_ShouldAddTheProject()
        {
            IGitRepository repo = CreateGitRepository( @"C:\TestRepo\" );
            ReleaseTagVersion releaseTagVersion = ReleaseTagVersion.TryParse( "v0.0.0" );
            IGitRepositoryVersion repoVersion = repo.CreateVersion( releaseTagVersion );
            ISolution solution = repoVersion.CreateSolution( "TestSolution1.sln" );
            IProject project1 = solution.CreateProject( @"P1\P1.xproj" );
            IProject project2 = solution.CreateProject( @"P2\P2.xproj" );
            IProjectDependency sut = project1.CreateNugetPackage( "Nunit", "2.4.6" );

            sut.AddProject( project2 );

            Assert.That( sut.Projects, Is.EquivalentTo( new[] { project1, project2 } ) );
            Assert.That( project1.Packages, Is.EquivalentTo( new[] { sut } ) );
            Assert.That( project2.Packages, Is.EquivalentTo( new[] { sut } ) );
        }

        protected abstract IGitRepository CreateGitRepository( string path );
    }
}
