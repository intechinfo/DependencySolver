using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Core
{
    public sealed class Solution : ISolution
    {
        readonly ItemContainer<string, Unit, IProject> _projectContainer;

        public Solution( IGitRepositoryVersion repoVersion, string path )
        {
            RepoVersion = repoVersion;
            Path = path;
            _projectContainer = new ItemContainer<string, Unit, IProject>(
                ( p, _ ) => new Project( this, p ),
                p => p.Path );
        }

        public IReadOnlyCollection<IProject> Projects => _projectContainer.Items;

        public IGitRepositoryVersion RepoVersion { get; }

        public string Path { get; }

        public IProject CreateProject( string path )
        {
            return _projectContainer.CreateItem( path, Unit.Value );
        }

        public void AddProject( IProject project )
        {
            if( project == null ) throw new ArgumentNullException( nameof( project ) );
            if( !ProjectIsOnRightContext( project ) ) throw new ArgumentException( CoreResources.ProjectBelongsToAnotherContext, nameof( project ) );
            _projectContainer.AddItem( project );
            if( !project.Solutions.Any( s => s == this ) ) project.AddSolution( this );
        }

        bool ProjectIsOnRightContext( IProject project )
        {
            Debug.Assert( project != null );
            if( project.Solutions == null ) return false;
            ISolution solution = project.Solutions.FirstOrDefault();
            if( solution == null ) return false;
            if( solution.RepoVersion == null ) return false;

            return solution.RepoVersion == RepoVersion;
        }
    }
}
