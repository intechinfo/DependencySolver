using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Core
{
    public class VPackage
    {
        public VPackage( VPackageId vPackageId )
            : this( vPackageId, new VPackage[ 0 ] )
        {
        }

        public VPackage( VPackageId vPackageId, IReadOnlyCollection<VPackage> dependencies )
            : this( vPackageId, dependencies, false )
        {
        }

        public VPackage( VPackageId vPackageId, bool isNotFound )
            : this( vPackageId, new VPackage[ 0 ], isNotFound )
        {
        }

        public VPackage( VPackageId vPackageId, IReadOnlyCollection<VPackage> dependencies, bool isNotFound )
        {
            VPackageId = vPackageId;
            Dependencies = dependencies;
            IsNotFound = isNotFound;
        }


        public VPackageId VPackageId { get; }

        public IReadOnlyCollection<VPackage> Dependencies { get; }

        public bool IsNotFound { get; }

        public override bool Equals( object obj )
        {
            VPackage other = obj as VPackage;
            return other != null
                && other.VPackageId == VPackageId
                && other.IsNotFound == IsNotFound
                && other.Dependencies.Intersect( Dependencies ).Count() == Dependencies.Count;
        }

        public override int GetHashCode()
        {
            int hashCode = VPackageId.GetHashCode() << 3 ^ IsNotFound.GetHashCode();
            foreach( VPackage dependency in Dependencies.OrderBy( p => p.VPackageId.Id ).ThenBy( p => p.VPackageId.Version ) )
            {
                hashCode = hashCode << 3 ^ dependency.GetHashCode();
            }
            return hashCode;
        }

        public static bool operator ==( VPackage p1, VPackage p2 )
        {
            return ReferenceEquals( p1, p2 ) || ( !ReferenceEquals( p1, null ) && p1.Equals( p2 ) );
        }

        public static bool operator !=( VPackage p1, VPackage p2 )
        {
            return !( p1 == p2 );
        }
    }
}
