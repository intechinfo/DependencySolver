using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Core
{
    public sealed class ProjectDependency : IProjectDependency
    {
        readonly List<IProject> _projects;

        public ProjectDependency( IProject project, string name, string version )
        {
            _projects = new List<IProject> { project };
            Name = name;
            Version = version;
        }

        public IReadOnlyCollection<IProject> Projects => _projects;

        public string Name { get; }

        public string Version { get; }

        public void AddProject( IProject project )
        {
            if( project == null ) throw new ArgumentNullException( nameof( project ) );
            if( !ProjectIsInRightContext( project ) ) throw new ArgumentException( CoreResources.ProjectBelongsToAnotherContext, nameof( project ) );
            _projects.Add( project );
            if( !project.Packages.Any( p => p == this ) ) project.AddNugetPackage( this );
        }

        bool ProjectIsInRightContext( IProject project )
        {
            Debug.Assert( project != null );
            if( project.Solutions == null ) return false;
            ISolution solution = project.Solutions.FirstOrDefault();
            if( solution == null ) return false;
            if( solution.RepoVersion == null || solution.RepoVersion.GitRepository == null ) return false;

            return solution.RepoVersion.GitRepository == _projects.First().Solutions.First().RepoVersion.GitRepository;
        }
    }
}