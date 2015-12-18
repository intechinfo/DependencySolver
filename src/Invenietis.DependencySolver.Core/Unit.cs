namespace Invenietis.DependencySolver.Core
{
    sealed class Unit
    {
        private Unit()
        {
        }

        internal static readonly Unit Value = new Unit();
    }
}
