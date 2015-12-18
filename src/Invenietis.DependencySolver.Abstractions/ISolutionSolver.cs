using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Abstractions
{
    public interface ISolutionSolver
    {
        void Solve( ISolution solution, string workingDirectoryPath, string slnPath );
    }
}