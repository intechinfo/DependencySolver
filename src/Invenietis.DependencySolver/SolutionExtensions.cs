using System.Linq;
using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Core
{
    public static class SolutionExtensions
    {
        public static bool AddOrCreateProject( this ISolution @this, string path, out IProject project )
        {
            project = @this.RepoVersion.Solutions
                .SelectMany( s => s.Projects )
                .FirstOrDefault( p => p.Path == path );

            if( project == null )
            {
                project = @this.CreateProject( path );
                return true;
            }

            @this.AddProject( project );
            return false;
        }
    }
}
