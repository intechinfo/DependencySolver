using System.IO;
using System.Xml.Linq;
using Invenietis.DependencySolver.Core;
using System.Linq;
using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.Abstractions;

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
                project.AddOrCreateNugetPackage( package.Id, package.Version, out p );
            }
        }
    }
}