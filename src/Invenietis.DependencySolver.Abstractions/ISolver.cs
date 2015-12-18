using Invenietis.DependencySolver.Core.Abstractions;

namespace Invenietis.DependencySolver.Abstractions
{
    public interface ISolver
    {
        IGitRepository Solve();
    }
}
