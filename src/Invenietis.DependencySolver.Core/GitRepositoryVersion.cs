using System.Collections.Generic;
using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.Util;
using SimpleGitVersion;

namespace Invenietis.DependencySolver.Core
{
    public sealed class GitRepositoryVersion : IGitRepositoryVersion
    {
        readonly ItemContainer<string, Unit, ISolution> _itemContainer;

        public GitRepositoryVersion( IGitRepository gitRepository, ReleaseTagVersion releaseTagVersion )
        {
            GitRepository = gitRepository;
            ReleaseTagVersion = releaseTagVersion;
            _itemContainer = new ItemContainer<string, Unit, ISolution>(
                ( n, _ ) => new Solution( this, n ) );
        }

        public IGitRepository GitRepository { get; }

        public ReleaseTagVersion ReleaseTagVersion { get; }

        public IReadOnlyCollection<ISolution> Solutions => _itemContainer.Items;

        public ISolution CreateSolution( string path )
        {
            if( string.IsNullOrWhiteSpace( path ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( path ) );
            return _itemContainer.CreateItem( path, Unit.Value );
        }
    }
}
