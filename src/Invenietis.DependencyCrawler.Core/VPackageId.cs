using Invenietis.DependencyCrawler.Util;

namespace Invenietis.DependencyCrawler.Core
{
    public class VPackageId
    {
        public VPackageId( string id, string version )
        {
            if( string.IsNullOrWhiteSpace( id ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( id ) );
            if( string.IsNullOrWhiteSpace( version ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( version ) );

            Id = id;
            Version = version;
        }

        public string Id { get; }

        public string Version { get; }

        public override bool Equals( object obj )
        {
            VPackageId other = obj as VPackageId;
            return other != null && other.Id == Id && other.Version == Version;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() << 3 ^ Version.GetHashCode();
        }
    }
}