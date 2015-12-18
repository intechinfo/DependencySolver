using System.Linq;
using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Core
{
    public static class ProjectExtensions
    {
        public static bool AddOrCreateNugetPackage( this IProject @this, string name, string version, out IProjectDependency package )
        {
            package = @this.Solutions.Select( s => s.RepoVersion.GitRepository )
                .SelectMany( r => r.RepoVersions )
                .SelectMany( v => v.Solutions )
                .SelectMany( s => s.Projects )
                .SelectMany( p => p.Packages )
                .FirstOrDefault( p => p.Name == name && p.Version == version );
            if( package == null )
            {
                @this.CreateNugetPackage( name, version );
                return true;
            }

            @this.AddNugetPackage( package );
            return false;
        }
    }
}
