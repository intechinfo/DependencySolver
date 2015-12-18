using System.Collections.Generic;

namespace Invenietis.DependencySolver.Core.Abstractions
{
    public interface ISolution
    {
        IGitRepositoryVersion RepoVersion { get; }

        string Path { get; }

        IProject CreateProject( string path );

        void AddProject( IProject project );

        IReadOnlyCollection<IProject> Projects { get; }
    }
}