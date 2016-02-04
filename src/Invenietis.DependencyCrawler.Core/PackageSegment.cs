using System;
using Invenietis.DependencyCrawler.Util;

namespace Invenietis.DependencyCrawler.Core
{
    public class PackageSegment
    {
        public PackageSegment( string packageManager, string start )
            : this( packageManager, start, string.Empty )
        {
        }

        public PackageSegment( string packageManager, string start, string end )
        {
            end = end ?? string.Empty;
            if( string.IsNullOrWhiteSpace( packageManager ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( packageManager ) );
            if( string.IsNullOrWhiteSpace( start ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( start ) );
            if( end != string.Empty && start.CompareTo( end ) > 0 ) throw new ArgumentException( CoreResources.StartMustBeLowerThanEnd );

            PackageManager = packageManager;
            Start = start;
            End = end;
        }

        public string PackageManager { get; }

        public string Start { get; }

        public string End { get; }

        public bool HasEnd => End != string.Empty;
    }
}
