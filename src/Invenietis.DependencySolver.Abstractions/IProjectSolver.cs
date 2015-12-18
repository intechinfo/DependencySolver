using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Abstractions
{
    public interface IProjectSolver
    {
        void Solve( IProject project, string projectPath );
    }
}