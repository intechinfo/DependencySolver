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
            return other != null && other.VPackageId.Equals( VPackageId ) && other.MaxVersion.Equals( MaxVersion );
        }

        public override int GetHashCode()
        {
            return VPackageId.GetHashCode() << 3 ^ MaxVersion.GetHashCode();
        }
    }
}