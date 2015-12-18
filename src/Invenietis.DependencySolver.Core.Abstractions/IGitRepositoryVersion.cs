using System.Collections.Generic;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core.Abstractions
{
    public interface IGitRepositoryVersion
    {
        IGitRepository GitRepository { get; }

        ReleaseTagVersion ReleaseTagVersion { get; }

        ISolution CreateSolution( string path );

        IReadOnlyCollection<ISolution> Solutions { get; }
    }
}
