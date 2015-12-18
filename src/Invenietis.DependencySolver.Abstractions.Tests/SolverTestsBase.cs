using System.Collections.Generic;
using System.Linq;
using Invenietis.DependencySolver.Core.Abstractions;
using NUnit.Framework;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Abstractions.Tests
{
    [TestFixture]
    public abstract class SolverTestsBase
    {
        [Test]
        public void Solve_WithVerySimpleRepo()
        {
            using( ZippedResource resourcePath = new ZippedResource( "Invenietis.DependencySolver.Abstractions.Tests.compiler.resources.TestProjects1.git.zip" ) )
            {
                ISolver sut = CreateSolver( resourcePath.Path );
                IGitRepository repo = sut.Solve();

                Assert.That( repo.RepoVersions.Count, Is.EqualTo( 1 ) );

                IGitRepositoryVersion repoVersion = repo.RepoVersions.Single();
                Assert.That( repoVersion.ReleaseTagVersion, Is.EqualTo( ReleaseTagVersion.TryParse( "v0.0.0" ) ) );
                Assert.That( repoVersion.Solutions.Count, Is.EqualTo( 1 ) );

                ISolution solution = repoVersion.Solutions.Single();
                Assert.That( solution.Path, Is.EqualTo( "TestProjects.sln" ) );
                Assert.That( solution.Projects.Count, Is.EqualTo( 1 ) );

                IProject project = solution.Projects.Single();
                Assert.That( project.Path, Is.EqualTo( @"src\TestProjects.P1\TestProjects.P1.xproj" ) );
                Assert.That( project.Packages.Count, Is.EqualTo( 1 ) );

                IProjectDependency package = project.Packages.Single();
                Assert.That( package.Name, Is.EqualTo( "NUnit" ) );
                Assert.That( package.Version, Is.EqualTo( "3.0.0" ) );
            }
        }

        [Test]
        public void Solve_WithRepoContaining3Tags()
        {
            using( ZippedResource resourcePath = new ZippedResource( "Invenietis.DependencySolver.Abstractions.Tests.compiler.resources.TestProjects2.git.zip" ) )
            {
                ISolver sut = CreateSolver( resourcePath.Path );
                IGitRepository repo = sut.Solve();

                ReleaseTagVersionComparer releaseTagVersionComparer = new ReleaseTagVersionComparer();
                List<IGitRepositoryVersion> repoVersions = repo.RepoVersions.OrderBy( r => r.ReleaseTagVersion, releaseTagVersionComparer ).ToList();
                Assert.That( repoVersions.Count, Is.EqualTo( 3 ) );

                IGitRepositoryVersion repoVersion1 = repoVersions[ 0 ];
                Assert.That( repoVersion1.ReleaseTagVersion, Is.EqualTo( ReleaseTagVersion.TryParse( "v0.0.0" ) ) );

                ISolution solution1 = repoVersion1.Solutions.Single();
                Assert.That( solution1.Path, Is.EqualTo( "TestProjects.sln" ) );

                IProject project1 = solution1.Projects.Single();
                Assert.That( project1.Path, Is.EqualTo( @"src\TestProjects.P1\TestProjects.P1.xproj" ) );

                IProjectDependency package1 = project1.Packages.Single();
                Assert.That( package1.Name, Is.EqualTo( "NUnit" ) );
                Assert.That( package1.Version, Is.EqualTo( "3.0.0" ) );

                IGitRepositoryVersion repoVersion2 = repoVersions[ 1 ];
                Assert.That( repoVersion2.ReleaseTagVersion, Is.EqualTo( ReleaseTagVersion.TryParse( "v0.1.0" ) ) );
                Assert.That( repoVersion2.Solutions.Count, Is.EqualTo( 2 ) );

                List<ISolution> solutions = repoVersion2.Solutions.OrderBy( s => s.Path ).ToList();
                ISolution solution2 = solutions[ 0 ];
                Assert.That( solution2.Path, Is.EqualTo( "TestProjects.P1.sln" ) );

                IProject project2 = solution2.Projects.Single();
                Assert.That( project2.Path, Is.EqualTo( @"src\TestProjects.P1\TestProjects.P1.xproj" ) );

                IProjectDependency package2 = project2.Packages.Single();
                Assert.That( package2, Is.SameAs( package1 ) );

                ISolution solution3 = solutions[ 1 ];
                Assert.That( solution3.Path, Is.EqualTo( "TestProjects.sln" ) );
                Assert.That( solution3.Projects.Count, Is.EqualTo( 2 ) );

                List<IProject> projects = solution3.Projects.OrderBy( p => p.Path ).ToList();
                IProject project3 = projects[ 0 ];
                Assert.That( project3, Is.SameAs( project2 ) );

                IProject project4 = projects[ 1 ];
                Assert.That( project4.Path, Is.EqualTo( @"src\TestProjects.P2\TestProjects.P2.xproj" ) );
                Assert.That( project4.Packages.Count, Is.EqualTo( 7 ) );

                List<IProjectDependency> packages1 = project4.Packages.OrderBy( p => p.Name ).ToList();
                Assert.That( packages1[ 0 ].Name, Is.EqualTo( "Microsoft.CSharp" ) );
                Assert.That( packages1[ 0 ].Version, Is.EqualTo( "4.0.1-beta-23516" ) );
                Assert.That( packages1[ 1 ].Name, Is.EqualTo( "Newtonsoft.Json" ) );
                Assert.That( packages1[ 1 ].Version, Is.EqualTo( "7.0.1" ) );
                Assert.That( packages1[ 2 ].Name, Is.EqualTo( "NUnit" ) );
                Assert.That( packages1[ 2 ].Version, Is.EqualTo( "3.0.1" ) );
                Assert.That( packages1[ 3 ].Name, Is.EqualTo( "System.Collections" ) );
                Assert.That( packages1[ 3 ].Version, Is.EqualTo( "4.0.11-beta-23516" ) );
                Assert.That( packages1[ 4 ].Name, Is.EqualTo( "System.Linq" ) );
                Assert.That( packages1[ 4 ].Version, Is.EqualTo( "4.0.1-beta-23516" ) );
                Assert.That( packages1[ 5 ].Name, Is.EqualTo( "System.Runtime" ) );
                Assert.That( packages1[ 5 ].Version, Is.EqualTo( "4.0.21-beta-23516" ) );
                Assert.That( packages1[ 6 ].Name, Is.EqualTo( "System.Threading" ) );
                Assert.That( packages1[ 6 ].Version, Is.EqualTo( "4.0.11-beta-23516" ) );

                IGitRepositoryVersion repoVersion3 = repoVersions[ 2 ];
                Assert.That( repoVersion3.ReleaseTagVersion, Is.EqualTo( ReleaseTagVersion.TryParse( "v0.2.0" ) ) );
                Assert.That( repoVersion3.Solutions.Count, Is.EqualTo( 2 ) );

                solutions = repoVersion3.Solutions.OrderBy( s => s.Path ).ToList();
                ISolution solution4 = solutions[ 0 ];
                Assert.That( solution4.Path, Is.EqualTo( "TestProjects.P1.sln" ) );

                IProject project5 = solution4.Projects.Single();
                Assert.That( project5.Path, Is.EqualTo( @"src\TestProjects.P1\TestProjects.P1.xproj" ) );

                IProjectDependency package3 = project5.Packages.Single();
                Assert.That( package3, Is.SameAs( package1 ) );

                ISolution solution5 = solutions[ 1 ];
                Assert.That( solution5.Path, Is.EqualTo( "TestProjects.sln" ) );
                Assert.That( solution5.Projects.Count, Is.EqualTo( 3 ) );

                projects = solution5.Projects.OrderBy( p => p.Path ).ToList();
                IProject project6 = projects[ 0 ];
                Assert.That( project6, Is.SameAs( project5 ) );

                IProject project7 = projects[ 1 ];
                Assert.That( project7.Path, Is.EqualTo( @"src\TestProjects.P2\TestProjects.P2.xproj" ) );
                Assert.That( project7.Packages.Count, Is.EqualTo( 7 ) );

                List<IProjectDependency> packages2 = project7.Packages.OrderBy( p => p.Name ).ToList();
                Assert.That( packages2[ 0 ], Is.SameAs( packages1[ 0 ] ) );
                Assert.That( packages2[ 1 ], Is.SameAs( packages1[ 1 ] ) );
                Assert.That( packages2[ 2 ], Is.SameAs( packages1[ 2 ] ) );
                Assert.That( packages2[ 3 ], Is.SameAs( packages1[ 3 ] ) );
                Assert.That( packages2[ 4 ], Is.SameAs( packages1[ 4 ] ) );
                Assert.That( packages2[ 5 ], Is.SameAs( packages1[ 5 ] ) );
                Assert.That( packages2[ 6 ], Is.SameAs( packages1[ 6 ] ) );

                IProject project8 = projects[ 2 ];
                Assert.That( project8.Path, Is.EqualTo( @"TestProjects.P3\TestProjects.P3.csproj" ) );
                Assert.That( project8.Packages.Count, Is.EqualTo( 2 ) );

                List<IProjectDependency> packages3 = project8.Packages.OrderBy( p => p.Name ).ToList();
                Assert.That( packages3[ 0 ].Name, Is.EqualTo( "EntityFramework" ) );
                Assert.That( packages3[ 0 ].Version, Is.EqualTo( "6.1.3" ) );
                Assert.That( packages3[ 1 ], Is.SameAs( packages1[ 1 ] ) );

                Assert.That( package1.Projects, Is.EquivalentTo( new[] { project1, project2, project5 } ) );
                Assert.That( project5.Solutions, Is.EquivalentTo( new[] { solution4, solution5 } ) );
            }
        }

        protected abstract ISolver CreateSolver( string url );

        class ReleaseTagVersionComparer : IComparer<ReleaseTagVersion>
        {
            public int Compare( ReleaseTagVersion x, ReleaseTagVersion y )
            {
                return x.CompareTo( y );
            }
        }
    }
}
