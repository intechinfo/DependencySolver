using System.IO;
using Invenietis.DependencySolver.Abstractions;
using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.Util;

namespace Invenietis.DependencySolver
{
    public sealed class RepoVersionSolver : IRepoVersionSolver
    {
        readonly ISolutionSolver _solutionSolver;

        public RepoVersionSolver( ISolutionSolver solutionSolver )
        {
            _solutionSolver = solutionSolver;
        }

        public void Solve( IGitRepositoryVersion repoVersion, string workingDirectoryPath )
        {
            foreach( string slnPath in Directory.EnumerateFiles( workingDirectoryPath, "*.sln", SearchOption.AllDirectories ) )
            {
                string solutionPath = FileUtil.RelativePath( workingDirectoryPath, slnPath );
                ISolution solution = repoVersion.CreateSolution( solutionPath );
                _solutionSolver.Solve( solution, workingDirectoryPath, slnPath );
            }
        }
    }
}