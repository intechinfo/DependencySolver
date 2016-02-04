namespace Invenietis.DependencyCrawler.Core
{
    public class PackageMaxVersion
    {
        public static readonly PackageMaxVersion Unknown = new PackageMaxVersion();

        PackageMaxVersion()
        {
        }

        public PackageMaxVersion( string releaseMaxVersion )
            : this( releaseMaxVersion, string.Empty )
        {
        }

        public PackageMaxVersion( string releaseMaxVersion, string preReleaseMaxVersion )
        {
            ReleaseMaxVersion = releaseMaxVersion;
            PreReleaseMaxVersion = preReleaseMaxVersion;
        }

        public string ReleaseMaxVersion { get; }

        public string PreReleaseMaxVersion { get; }

        public bool HasPreReleaseMaxVersion
        {
            get { return PreReleaseMaxVersion != string.Empty; }
        }

        public bool HasReleaseMaxVersion
        {
            get { return ReleaseMaxVersion != string.Empty; }
        }

        public override bool Equals( object obj )
        {
            PackageMaxVersion other = obj as PackageMaxVersion;
            return other != null
                && other.ReleaseMaxVersion == ReleaseMaxVersion
                && other.PreReleaseMaxVersion == PreReleaseMaxVersion;
        }

        public override int GetHashCode()
        {
            return ReleaseMaxVersion.GetHashCode() << 7 ^ PreReleaseMaxVersion.GetHashCode();
        }

        public static bool operator ==( PackageMaxVersion v1, PackageMaxVersion v2 )
        {
            return ReferenceEquals( v1, v2 ) || ( !ReferenceEquals( v1, null ) && v1.Equals( v2 ) );
        }

        public static bool operator !=( PackageMaxVersion v1, PackageMaxVersion v2 )
        {
            return !( v1 == v2 );
        }
    }
}
