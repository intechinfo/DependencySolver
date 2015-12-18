using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.Core.Abstractions.Tests;
using NUnit.Framework;

namespace Invenietis.DependencySolver.Core.Tests
{
    [TestFixture]
    public class RepoVersionTests : RepoVersionTestsBase
    {
        protected override IGitRepository CreateGitRepository( string path )
        {
            return new GitRepository( path );
        }
    }
}
