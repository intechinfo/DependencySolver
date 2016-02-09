using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Core
{
    public class VPackage
    {
        public VPackage( VPackageId vPackageId )
            : this( vPackageId, new Platform[ 0 ] )
        {
        }

        public VPackage( VPackageId vPackageId, IReadOnlyCollection<VPackage> dependencies )
            : this( vPackageId, dependencies, false )
        {
        }

        public VPackage( VPackageId vPackageId, IReadOnlyCollection<Platform> platforms )
            : this( vPackageId, platforms, false )
        {
        }

        public VPackage( VPackageId vPackageId, bool isNotFound )
            : this( vPackageId, new Platform[ 0 ], isNotFound )
        {
        }

        public VPackage( VPackageId vPackageId, IReadOnlyCollection<VPackage> dependencies, bool isNotFound )
            : this( vPackageId, new[] { new Platform( dependencies ) }, isNotFound )
        {
        }

        public VPackage( VPackageId vPackageId, IReadOnlyCollection<Platform> platforms, bool isNotFound )
        {
            VPackageId = vPackageId;
            Platforms = platforms;
            IsNotFound = isNotFound;
        }


        public VPackageId VPackageId { get; }

        public IReadOnlyCollection<Platform> Platforms { get; }

        public bool IsNotFound { get; }

        public override bool Equals( object obj )
        {
            VPackage other = obj as VPackage;
            bool result = other != null
                && other.VPackageId == VPackageId
                && other.IsNotFound == IsNotFound
                && other.Platforms.Intersect( Platforms ).Count() == Platforms.Count;

            return result;
        }

        public override int GetHashCode()
        {
            int hashCode = VPackageId.GetHashCode() << 3 ^ IsNotFound.GetHashCode();
            foreach( Platform dependency in Platforms.OrderBy( p => p.PlatformId ) )
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
