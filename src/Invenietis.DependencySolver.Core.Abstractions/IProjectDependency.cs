using System.Collections.Generic;

namespace Invenietis.DependencySolver.Core.Abstractions
{
    public interface IProjectDependency
    {
        IReadOnlyCollection<IProject> Projects { get; }

        string Name { get; }

        string Version { get; }

        void AddProject( IProject project );
    }
}
