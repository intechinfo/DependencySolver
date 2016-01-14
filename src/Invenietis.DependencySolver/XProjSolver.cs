using System.IO;
using Invenietis.DependencySolver.Abstractions;
using Invenietis.DependencySolver.Core;
using Invenietis.DependencySolver.Core.Abstractions;
using Invenietis.DependencySolver.JSon;

namespace Invenietis.DependencySolver
{
    public sealed class XProjSolver : IProjectSolver
    {
        public void Solve( IProject project, string projectPath )
        {
            FileInfo projectFileInfo = new FileInfo( projectPath );
            string jsonProject = File.ReadAllText( Path.Combine( projectFileInfo.Directory.FullName, "project.json" ) );
            StringMatcher matcher = new StringMatcher( jsonProject );
            DependencyFinder dependencyFinder = new DependencyFinder( matcher );
            foreach( var dependency in dependencyFinder.Dependencies )
            {
                IProjectDependency p;
                project.AddOrCreateProjectDependency( dependency.Key, dependency.Value, out p );
            }
        }
    }
}