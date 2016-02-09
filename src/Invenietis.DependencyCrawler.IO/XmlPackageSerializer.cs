using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.IO
{
    public class XmlPackageSerializer : IPackageSerializer
    {
        public VPackage DeserializeVPackage( string serializedVPackage )
        {
            XElement xElement = XElement.Parse( serializedVPackage );
            VPackageId vPackageId = VPackageIdFromXElement( xElement );

            Dictionary<VPackageId, IEnumerable<VPackageId>> dependencies =
                xElement.Elements( "VPackageInfo" )
                        .Select( x => new { PackageId = VPackageIdFromXElement( x ), Dependencies = DependenciesFromXElement( x ) } )
                        .ToDictionary( x => x.PackageId, x => x.Dependencies );

            return BuildVPackage( vPackageId, dependencies );
        }

        VPackage BuildVPackage( VPackageId vPackageId, Dictionary<VPackageId, IEnumerable<VPackageId>> dependenciesDict )
        {
            return BuildVPackage( vPackageId, dependenciesDict, new Dictionary<VPackageId, VPackage>() );
        }

        VPackage BuildVPackage(
            VPackageId vPackageId,
            Dictionary<VPackageId, IEnumerable<VPackageId>> dependenciesDict,
            Dictionary<VPackageId, VPackage> cache )
        {
            VPackage cached;
            if( cache.TryGetValue( vPackageId, out cached ) ) return cached;

            IReadOnlyCollection<VPackage> dependencies =
                dependenciesDict[ vPackageId ].Select( x => BuildVPackage( x, dependenciesDict, cache ) )
                                              .ToList();

            VPackage result = new VPackage( vPackageId, dependencies );
            cache.Add( vPackageId, result );
            return result;
        }

        IEnumerable<VPackageId> DependenciesFromXElement( XElement xElement )
        {
            return xElement.Element( "Dependencies" ).Elements( "Dependency" ).Select( VPackageIdFromXElement );
        }

        VPackageId VPackageIdFromXElement(XElement xElement)
        {
            return new VPackageId(
                xElement.Attribute( "PackageManager" ).Value,
                xElement.Attribute( "Id" ).Value,
                xElement.Attribute( "Version" ).Value );
        }

        public string Serialize( VPackage vPackage )
        {
            XElement xElement = new XElement( "VPackage",
                new XAttribute( "PackageManager", vPackage.VPackageId.PackageManager ),
                new XAttribute( "Id", vPackage.VPackageId.Id ),
                new XAttribute( "Version", vPackage.VPackageId.Version ),
                XVPackageInfosFromVPackage( vPackage ) );

            return xElement.ToString();
        }

        XElement XPackageInfoFromVPackage( VPackage vPackage )
        {
            return new XElement( "VPackageInfo",
                new XAttribute( "PackageManager", vPackage.VPackageId.PackageManager ),
                new XAttribute( "Id", vPackage.VPackageId.Id ),
                new XAttribute( "Version", vPackage.VPackageId.Version ),
                new XElement( "Dependencies", XDependenciesFromVPackage( vPackage ) ) );
        }

        IEnumerable<XElement> XVPackageInfosFromVPackage( VPackage vPackage )
        {
            Dictionary<VPackageId, VPackage> toSerialize = new Dictionary<VPackageId, VPackage>();

            Action<VPackage> fill = null;
            fill = p =>
            {
                if( !toSerialize.ContainsKey( p.VPackageId ) )
                {
                    toSerialize.Add( p.VPackageId, p );
                    foreach( VPackage dependency in p.Dependencies )
                    {
                        fill( dependency );
                    }
                }
            };

            fill( vPackage );

            return toSerialize.Select( x => XPackageInfoFromVPackage( x.Value ) );
        }

        IEnumerable<XElement> XDependenciesFromVPackage( VPackage vPackage )
        {
            return vPackage.Dependencies.Select( d =>
                new XElement( "Dependency",
                    new XAttribute( "PackageManager", d.VPackageId.PackageManager ),
                    new XAttribute( "Id", d.VPackageId.Id ),
                    new XAttribute( "Version", d.VPackageId.Version ) ) );
        }
    }
}
