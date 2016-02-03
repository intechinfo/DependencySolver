namespace Invenietis.DependencyCrawler.Core
{
    public class PackageMaxVersion
    {
        public static readonly PackageMaxVersion Unknown = new PackageMaxVersion();

        PackageMaxVersion()
        {
        }

        public PackageMaxVersion( string maxVersion )
            : this( maxVersion, string.Empty )
        {
        }

        public PackageMaxVersion( string maxVersion, string preReleaseMaxVersion )
        {
            MaxVersion = maxVersion;
            PreReleaseMaxVersion = preReleaseMaxVersion;
        }

        public string MaxVersion { get; }

        public string PreReleaseMaxVersion { get; }

        public bool HasPreReleaseMaxVersion
        {
            get { return PreReleaseMaxVersion != string.Empty; }
        }

        public bool HasReleaseMaxVersion
        {
            get { return MaxVersion != string.Empty; }
        }

        public override bool Equals( object obj )
        {
            PackageMaxVersion other = obj as PackageMaxVersion;
            return other != null && other.MaxVersion == MaxVersion && other.PreReleaseMaxVersion == PreReleaseMaxVersion;
        }

        public override int GetHashCode()
        {
            return MaxVersion.GetHashCode() << 7 ^ PreReleaseMaxVersion.GetHashCode();
        }
    }
}
