using Invenietis.DependencyCrawler.Util;

namespace Invenietis.DependencyCrawler.Core
{
    public class PackageId
    {
        public PackageId( string id )
        {
            if( string.IsNullOrWhiteSpace( id ) ) ExceptionHelpers.ArgumentException( CoreResources.MustBeNotNullNorWhiteSpace, nameof( id ) );

            Id = id;
        }

        public string Id { get; }

        public override bool Equals( object obj )
        {
            PackageId other = obj as PackageId;
            return other != null && other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}