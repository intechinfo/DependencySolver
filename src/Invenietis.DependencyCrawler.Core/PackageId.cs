using Invenietis.DependencyCrawler.Util;

namespace Invenietis.DependencyCrawler.Core
{
    public class PackageId
    {
        public static readonly string NuGet = "NuGet";

        public PackageId( string packageManager, string value )
        {
            if( string.IsNullOrWhiteSpace( packageManager ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( packageManager ) );
            if( string.IsNullOrWhiteSpace( value ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( value ) );

            Value = value;
            PackageManager = packageManager;
        }

        public string Value { get; }

        public string PackageManager { get; }

        public override bool Equals( object obj )
        {
            PackageId other = obj as PackageId;
            return !ReferenceEquals( other, null )
                && other.Value == Value
                && other.PackageManager == PackageManager;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public static bool operator ==( PackageId p1, PackageId p2 )
        {
            return ReferenceEquals( p1, p2 ) || ( !ReferenceEquals( p1, null ) && p1.Equals( p2 ) );
        }

        public static bool operator !=( PackageId p1, PackageId p2 )
        {
            return !( p1 == p2 );
        }
    }
}