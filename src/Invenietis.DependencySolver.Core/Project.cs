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
        readonly ItemContainer<string, string, IProjectDependency> _packageContainer;
        readonly Dictionary<string, ISolution> _solutions;

        public Project( ISolution solution, string path )
        {
            _solutions = new Dictionary<string, ISolution> { { solution.Path, solution } };
            Path = path;
            _packageContainer = new ItemContainer<string, string, IProjectDependency>(
                ( id, v ) => new ProjectDependency( this, id, v ),
                p => p.Name );
        }

        public string Path { get; }

        public IReadOnlyCollection<IProjectDependency> Packages => _packageContainer.Items;

        public IReadOnlyCollection<ISolution> Solutions => _solutions.Values.ToList();

        public IProjectDependency CreateNugetPackage( string name, string version )
        {
            if( string.IsNullOrWhiteSpace( name ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( name ) );
            if( string.IsNullOrWhiteSpace( version ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( version ) );
            return _packageContainer.CreateItem( name, version );
        }

        public void AddNugetPackage( IProjectDependency nugetPackage )
        {
            if( nugetPackage == null ) throw new ArgumentNullException( nameof( nugetPackage ) );
            if( !PackageIsInRightContext( nugetPackage ) ) throw new ArgumentException( CoreResources.PackageBelongsToAnotherContext, nameof( nugetPackage ) );
            _packageContainer.AddItem( nugetPackage );
            if( !nugetPackage.Projects.Any( p => p == this ) ) nugetPackage.AddProject( this );
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

        bool PackageIsInRightContext( IProjectDependency package )
        {
            Debug.Assert( package != null );
            if( package.Projects == null ) return false;
            IProject project = package.Projects.FirstOrDefault();
            if( project == null || project.Solutions == null ) return false;
            ISolution solution = project.Solutions.FirstOrDefault();

            return solution != null && SolutionIsInRightContext( solution );
        }
    }
}