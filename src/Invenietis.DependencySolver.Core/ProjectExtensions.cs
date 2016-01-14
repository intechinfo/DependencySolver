using System.Linq;
using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Core
{
    public static class ProjectExtensions
    {
        public static bool AddOrCreateProjectDependency( this IProject @this, string name, string version, out IProjectDependency dependency )
        {
            dependency = @this.Solutions.Select( s => s.RepoVersion.GitRepository )
                .SelectMany( r => r.RepoVersions )
                .SelectMany( v => v.Solutions )
                .SelectMany( s => s.Projects )
                .SelectMany( p => p.Dependencies )
                .FirstOrDefault( p => p.Name == name && p.Version == version );
            if( dependency == null )
            {
                @this.CreateDependency( name, version );
                return true;
            }

            @this.AddDependency( dependency );
            return false;
        }
    }
}
