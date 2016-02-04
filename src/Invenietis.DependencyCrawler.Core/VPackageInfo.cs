using Invenietis.DependencyCrawler.Util;

namespace Invenietis.DependencyCrawler.Core
{
    public class VPackageInfo
    {
        public VPackageInfo( VPackageId vPackageId, PackageMaxVersion maxVersion )
        {
            VPackageId = vPackageId;
            MaxVersion = maxVersion;
        }

        public VPackageId VPackageId { get; }

        public PackageMaxVersion MaxVersion { get; }

        public override bool Equals( object obj )
        {
            VPackageInfo other = obj as VPackageInfo;
            return other != null
                && other.VPackageId == VPackageId
                && other.MaxVersion == MaxVersion;
        }

        public override int GetHashCode()
        {
            return VPackageId.GetHashCode() << 3 ^ MaxVersion.GetHashCode();
        }

        public static bool operator ==(VPackageInfo i1, VPackageInfo i2)
        {
            return ReferenceEquals( i1, i2 ) || ( !ReferenceEquals( i1, null ) && i1.Equals( i2 ) );
        }

        public static bool operator !=( VPackageInfo i1, VPackageInfo i2 )
        {
            return !( i1 == i2 );
        }
    }
}