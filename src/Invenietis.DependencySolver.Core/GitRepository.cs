using System;
using System.Collections.Generic;
using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.Util;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core
{
    public sealed class GitRepository : IGitRepository
    {
        readonly ItemContainer<ReleaseTagVersion, Unit, IGitRepositoryVersion> _repoVersionsContainer;

        public GitRepository( string path )
        {
            Path = path.NormalizeDirectory();
            _repoVersionsContainer = new ItemContainer<ReleaseTagVersion, Unit, IGitRepositoryVersion>(
                ( id, _ ) => new GitRepositoryVersion( this, id ) );
        }

        public string Path { get; }

        public IReadOnlyCollection<IGitRepositoryVersion> RepoVersions => _repoVersionsContainer.Items;

        public IGitRepositoryVersion CreateVersion( ReleaseTagVersion v )
        {
            if( !v.IsValid ) throw new ArgumentException( CoreResources.TagMustBeValid );
            return _repoVersionsContainer.CreateItem( v, Unit.Value );
        }
    }
}