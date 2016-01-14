using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.Util;

namespace Invenietis.DependencySolver.Core
{
    public sealed class Project : IProject
    {
        readonly ItemContainer<string, string, IProjectDependency> _dependencyContainer;
        readonly Dictionary<string, ISolution> _solutions;

        public Project( ISolution solution, string path )
        {
            _solutions = new Dictionary<string, ISolution> { { solution.Path, solution } };
            Path = path;
            _dependencyContainer = new ItemContainer<string, string, IProjectDependency>(
                ( id, v ) => new ProjectDependency( this, id, v ),
                p => p.Name );
        }

        public string Path { get; }

        public IReadOnlyCollection<IProjectDependency> Dependencies => _dependencyContainer.Items;

        public IReadOnlyCollection<ISolution> Solutions => _solutions.Values.ToList();

        public IProjectDependency CreateDependency( string name, string version )
        {
            if( string.IsNullOrWhiteSpace( name ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( name ) );
            if( string.IsNullOrWhiteSpace( version ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( version ) );
            return _dependencyContainer.CreateItem( name, version );
        }

        public void AddDependency( IProjectDependency dependency )
        {
            if( dependency == null ) throw new ArgumentNullException( nameof( dependency ) );
            if( !DependencyIsInRightContext( dependency ) ) throw new ArgumentException( CoreResources.DependencyBelongsToAnotherContext, nameof( dependency ) );
            _dependencyContainer.AddItem( dependency );
            if( !dependency.Projects.Any( p => p == this ) ) dependency.AddProject( this );
        }

        public void AddSolution( ISolution solution )
        {
            if( !SolutionIsInRightContext( solution ) ) throw new ArgumentException( CoreResources.SolutionBelongsToAnotherContext, nameof( solution ) );

            _solutions.Add( solution.Path, solution );
            if( !solution.Projects.Any( p => p == this ) ) solution.AddProject( this );
        }

        bool SolutionIsInRightContext( ISolution solution )
        {
            Debug.Assert( solution != null );
            if( solution.RepoVersion == null || solution.RepoVersion.GitRepository == null ) return false;

            return solution.RepoVersion.GitRepository == Solutions.First().RepoVersion.GitRepository;
        }

        bool DependencyIsInRightContext( IProjectDependency dependency )
        {
            Debug.Assert( dependency != null );
            if( dependency.Projects == null ) return false;
            IProject project = dependency.Projects.FirstOrDefault();
            if( project == null || project.Solutions == null ) return false;
            ISolution solution = project.Solutions.FirstOrDefault();

            return solution != null && SolutionIsInRightContext( solution );
        }
    }
}