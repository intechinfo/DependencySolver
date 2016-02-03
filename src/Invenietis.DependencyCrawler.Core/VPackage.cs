using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Core
{
    public class VPackage
    {
        public VPackage( VPackageId vPackageId )
            : this( vPackageId, new VPackageId[ 0 ] )
        {
        }

        public VPackage( VPackageId vPackageId, IReadOnlyCollection<VPackageId> dependencies )
            : this(vPackageId, dependencies, false )
        {
        }

        public VPackage( VPackageId vPackageId, bool isNotFound )
            : this( vPackageId, new VPackageId[ 0 ], isNotFound )
        {
        }

        public VPackage( VPackageId vPackageId, IReadOnlyCollection<VPackageId> dependencies, bool isNotFound )
        {
            VPackageId = vPackageId;
            Dependencies = dependencies;
            IsNotFound = isNotFound;
        }


        public VPackageId VPackageId { get; }

        public IReadOnlyCollection<VPackageId> Dependencies { get; }

        public bool IsNotFound { get; }

        public override bool Equals( object obj )
        {
            VPackage other = obj as VPackage;
            return other != null
                && other.VPackageId.Equals( VPackageId )
                && other.IsNotFound == IsNotFound
                && other.Dependencies.Intersect( Dependencies ).Count() == Dependencies.Count;
        }

        public override int GetHashCode()
        {
            int hashCode = VPackageId.GetHashCode() << 3 ^ IsNotFound.GetHashCode();
            foreach( VPackageId dependency in Dependencies.OrderBy( p => p.Version ).ThenBy( p => p.Id ) )
            {
                hashCode = hashCode << 3 ^ dependency.GetHashCode();
            }
            return hashCode;
        }
    }
}
