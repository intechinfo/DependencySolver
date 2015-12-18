using Invenietis.DependencySolver.Abstractions;
using Invenietis.DependencySolver.Abstractions.Tests;
using Invenietis.DependencySolver.Core;
using NUnit.Framework;

namespace Invenietis.DependencySolver.Tests
{
    [TestFixture]
    public class SolverTests : SolverTestsBase
    {
        protected override ISolver CreateSolver( string url )
        {
            return SolverHelper.CreateNew( s => new GitRepository( s ), url );
        }
    }
}
