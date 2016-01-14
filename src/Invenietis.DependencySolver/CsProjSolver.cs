using System.IO;
using System.Linq;
using System.Xml.Linq;
using Invenietis.DependencySolver.Abstractions;
using Invenietis.DependencySolver.Core;
using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver
{
    public sealed class CsProjSolver : IProjectSolver
    {
        public void Solve( IProject project, string projectPath )
        {
            string packageConfPath = Path.Combine( Path.GetDirectoryName( projectPath ), "packages.config" );
            if( !File.Exists( packageConfPath ) ) return;
            XElement packageElement = XElement.Load( packageConfPath );
            var packages = packageElement.Descendants( "package" )
                .Select( p => new { Id = p.Attribute( "id" ).Value, Version = p.Attribute( "version" ).Value } );
            foreach( var package in packages )
            {
                IProjectDependency p;
                project.AddOrCreateProjectDependency( package.Id, package.Version, out p );
            }
        }
    }
}