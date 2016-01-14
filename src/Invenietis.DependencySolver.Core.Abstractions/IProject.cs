using System.Collections.Generic;

namespace Invenietis.DependencySolver.Core.Abstractions
{
    public interface IProject
    {
        IReadOnlyCollection<ISolution> Solutions { get; }

        string Path { get; }

        IProjectDependency CreateDependency( string name, string version );

        void AddDependency( IProjectDependency dependency );

        IReadOnlyCollection<IProjectDependency> Dependencies { get; }

        void AddSolution( ISolution solution );
    }
}
