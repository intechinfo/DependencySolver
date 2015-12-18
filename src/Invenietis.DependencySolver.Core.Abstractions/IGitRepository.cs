using System.Collections.Generic;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core.Abstractions
{
    public interface IGitRepository
    {
        IReadOnlyCollection<IGitRepositoryVersion> RepoVersions { get; }

        IGitRepositoryVersion CreateVersion( ReleaseTagVersion v );
    }
}