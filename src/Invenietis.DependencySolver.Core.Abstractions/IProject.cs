using System.Collections.Generic;

namespace Invenietis.DependencySolver.Core.Abstractions
{
    public interface IProject
    {
        IReadOnlyCollection<ISolution> Solutions { get; }

        string Path { get; }

        IProjectDependency CreateNugetPackage( string name, string version );

        void AddNugetPackage( IProjectDependency package );

        IReadOnlyCollection<IProjectDependency> Packages { get; }

        void AddSolution( ISolution solution );
    }
}
