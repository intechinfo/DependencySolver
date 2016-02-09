using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Core
{
    public class PackageInfo
    {
        public PackageInfo( VPackageId packageInfo )
            : this( packageInfo, null )
        {
        }

        public PackageInfo( VPackageId packageInfo, IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>> dependencies )
        {
            if( packageInfo == null ) throw new ArgumentNullException( nameof( packageInfo ) );
            if( dependencies == null ) dependencies = new Dictionary<PlatformId, IEnumerable<VPackageId>>();
            VPackageId = packageInfo;
            Dependencies = dependencies;
        }

        public VPackageId VPackageId { get; }

        public IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>> Dependencies { get; }
    }
}
