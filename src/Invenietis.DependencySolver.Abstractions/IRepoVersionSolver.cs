using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Abstractions
{
    public interface IRepoVersionSolver
    {
        void Solve( IGitRepositoryVersion repoVersion, string workingDirectoryPath );
    }
}