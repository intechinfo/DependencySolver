using System;

namespace Invenietis.DependencySolver.Util
{
    public static class ExceptionHelpers
    {
        public static void ArgumentException( string format, string paramName )
        {
            throw new ArgumentException( string.Format( format, paramName ), paramName );
        }
    }
}
