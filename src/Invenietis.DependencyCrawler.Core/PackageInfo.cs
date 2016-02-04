using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Core
{
    public class PackageInfo
    {
        public PackageInfo( VPackageId packageInfo )
            : this( packageInfo, new VPackageId[ 0 ] )
        {
        }

        public PackageInfo( VPackageId packageInfo, IReadOnlyCollection<VPackageId> dependencies )
        {
            if( packageInfo == null ) throw new ArgumentNullException( nameof( packageInfo ) );
            if( dependencies == null ) throw new ArgumentNullException( nameof( dependencies ) );

            VPackageId = packageInfo;
            Dependencies = dependencies;
        }

        public VPackageId VPackageId { get; }

        public IReadOnlyCollection<VPackageId> Dependencies { get; }
    }
}
