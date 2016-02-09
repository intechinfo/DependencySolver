namespace Invenietis.DependencyCrawler.Core
{
    public class PlatformId
    {
        public static readonly PlatformId None = new PlatformId( string.Empty );

        public PlatformId( string value )
        {
            if( string.IsNullOrWhiteSpace( value ) ) value = string.Empty;
            Value = value;
        }

        public string Value { get; }

        public override bool Equals( object obj )
        {
            PlatformId other = obj as PlatformId;
            return other != null && other.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==( PlatformId p1, PlatformId p2 )
        {
            return ReferenceEquals( p1, p2 ) || ( !ReferenceEquals( p1, null ) && p1.Equals( p2 ) );
        }

        public static bool operator !=( PlatformId p1, PlatformId p2 )
        {
            return !( p1 == p2 );
        }
    }
}
