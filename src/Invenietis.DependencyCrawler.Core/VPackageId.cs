using Invenietis.DependencyCrawler.Util;

namespace Invenietis.DependencyCrawler.Core
{
    public class VPackageId
    {
        public VPackageId( string packageManager, string id, string version )
        {
            if( string.IsNullOrWhiteSpace( packageManager ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( packageManager ) );
            if( string.IsNullOrWhiteSpace( id ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( id ) );
            if( string.IsNullOrWhiteSpace( version ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( version ) );

            PackageManager = packageManager;
            Id = id;
            Version = version;
        }

        public string PackageManager { get; }

        public string Id { get; }

        public string Version { get; }

        public override bool Equals( object obj )
        {
            VPackageId other = obj as VPackageId;
            return other != null
                && other.Id == Id
                && other.Version == Version
                && other.PackageManager == PackageManager;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() << 3 ^ Version.GetHashCode();
        }

        public static bool operator ==( VPackageId p1, VPackageId p2 )
        {
            return ReferenceEquals( p1, p2 ) || ( !ReferenceEquals( p1, null ) && p1.Equals( p2 ) );
        }

        public static bool operator !=( VPackageId p1, VPackageId p2 )
        {
            return !( p1 == p2 );
        }
    }
}