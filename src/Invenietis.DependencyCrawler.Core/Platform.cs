using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Core
{
    public class Platform
    {
        public Platform( IReadOnlyCollection<VPackage> vPackages )
            : this( PlatformId.None, vPackages )
        {
        }

        public Platform( PlatformId platformId )
            : this( platformId, new VPackage[ 0 ] )
        {
        }

        public Platform( PlatformId platformId, IReadOnlyCollection<VPackage> vPackages )
        {
            PlatformId = platformId;
            VPackages = vPackages;
        }

        public PlatformId PlatformId { get; }

        public IReadOnlyCollection<VPackage> VPackages { get; }

        public override bool Equals( object obj )
        {
            Platform other = obj as Platform;
            return other != null
                && other.PlatformId == PlatformId
                && other.VPackages.Intersect( VPackages ).Count() == VPackages.Count;
        }

        public override int GetHashCode()
        {
            int hashCode = PlatformId.GetHashCode();
            foreach( VPackage vPackage in VPackages )
            {
                hashCode = ( hashCode << 3 ) ^ vPackage.GetHashCode();
            }
            return hashCode;
        }

        public static bool operator ==( Platform p1, Platform p2 )
        {
            return ReferenceEquals( p1, p2 ) || ( !ReferenceEquals( p1, null ) && p1.Equals( p2 ) );
        }

        public static bool operator !=( Platform p1, Platform p2 )
        {
            return !( p1 == p2 );
        }
    }
}
