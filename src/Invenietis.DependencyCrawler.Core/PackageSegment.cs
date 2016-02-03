using System;
using Invenietis.DependencyCrawler.Util;

namespace Invenietis.DependencyCrawler.Core
{
    public class PackageSegment
    {
        public PackageSegment( string start, string end )
        {
            if( string.IsNullOrWhiteSpace( start ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( start ) );
            if( string.IsNullOrWhiteSpace( end ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( end ) );
            if( start.CompareTo( end ) > 0 ) throw new ArgumentException( CoreResources.StartMustBeLowerThanEnd );

            Start = start;
            End = end;
        }

        public string Start { get; }

        public string End { get; }

        public bool Contains( string s )
        {
            return Start.ToLower().CompareTo( s.ToLower() ) <= 0 && s.ToLower().CompareTo( End.ToLower() ) <= 0;
        }
    }
}
