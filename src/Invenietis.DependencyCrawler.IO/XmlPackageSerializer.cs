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

            Dictionary<VPackageId, Dictionary<PlatformId, IEnumerable<VPackageId>>> platforms =
                xElement.Elements( "VPackageInfo" )
                    .Select( x => new
                    {
                        PackageId = VPackageIdFromXElement( x ),
                        Platforms = PlatformsFromXElement( x )
                    } )
                    .ToDictionary( x => x.PackageId, x => x.Platforms );

            return BuildVPackage( vPackageId, platforms );
        }

        VPackage BuildVPackage(
            VPackageId vPackageId,
            Dictionary<VPackageId, Dictionary<PlatformId, IEnumerable<VPackageId>>> dependenciesDict )
        {
            return BuildVPackage( vPackageId, dependenciesDict, new Dictionary<VPackageId, VPackage>() );
        }

        VPackage BuildVPackage(
            VPackageId vPackageId,
            Dictionary<VPackageId, Dictionary<PlatformId, IEnumerable<VPackageId>>> dependenciesDict,
            Dictionary<VPackageId, VPackage> cache )
        {
            VPackage cached;
            if( cache.TryGetValue( vPackageId, out cached ) ) return cached;

            IReadOnlyCollection<Platform> platforms =
                dependenciesDict[ vPackageId ]
                    .Select( x => new Platform( x.Key, x.Value.Select( p => BuildVPackage( p, dependenciesDict, cache ) )
                    .ToList() ) ).ToList();

            VPackage result = new VPackage( vPackageId, platforms );
            cache.Add( vPackageId, result );
            return result;
        }

        Dictionary<PlatformId, IEnumerable<VPackageId>> PlatformsFromXElement( XElement xElement )
        {
            return xElement
                .Elements( "Platform" )
                .Select( x => new
                {
                    Id = new PlatformId( x.Attribute( "Id" ).Value ),
                    Dependencies = x.Elements( "Dependency" ).Select( VPackageIdFromXElement )
                } )
                .ToDictionary( x => x.Id, x => x.Dependencies );
        }

        VPackageId VPackageIdFromXElement( XElement xElement )
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
                XPlatformsFromVPackage( vPackage ) );
        }

        IEnumerable<XElement> XVPackageInfosFromVPackage( VPackage vPackage )
        {
            Dictionary<VPackageId, VPackage> toSerialize = new Dictionary<VPackageId, VPackage>();

            Action<VPackage> fillVPackage = null;
            fillVPackage = p =>
            {
                if( !toSerialize.ContainsKey( p.VPackageId ) )
                {
                    toSerialize.Add( p.VPackageId, p );
                    foreach( Platform platform in p.Platforms )
                    {
                        foreach( VPackage vp in platform.VPackages ) fillVPackage( vp );
                    }
                }
            };

            fillVPackage( vPackage );

            return toSerialize.Select( x => XPackageInfoFromVPackage( x.Value ) );
        }

        IEnumerable<XElement> XPlatformsFromVPackage( VPackage vPackage )
        {
            return vPackage.Platforms.Select( p => new XElement( "Platform",
                new XAttribute( "Id", p.PlatformId.Value ),
                p.VPackages.Select( v => new XElement( "Dependency",
                    new XAttribute( "PackageManager", v.VPackageId.PackageManager ),
                    new XAttribute( "Id", v.VPackageId.Id ),
                    new XAttribute( "Version", v.VPackageId.Version ) ) ) ) );
        }

        public string Serialize( IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>> dependencies )
        {
            if( dependencies == null ) dependencies = new Dictionary<PlatformId, IEnumerable<VPackageId>>();
            return new XElement(
                "Platforms",
                dependencies.Select( d => new XElement(
                     "Platform",
                     new XAttribute( "Id", d.Key.Value ),
                     d.Value.Select( x => new XElement(
                         "Dependency",
                         new XAttribute( "PackageManager", x.PackageManager ),
                         new XAttribute( "Id", x.Id ),
                         new XAttribute( "Version", x.Version ) ) ) ) ) ).ToString();
        }

        public IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>> DeserializeVPackageDependencies( string serializedVPackageDependencies )
        {
            return XElement.Parse( serializedVPackageDependencies )
                .Elements( "Platform" )
                .Select( x => new
                {
                    Id = new PlatformId( x.Attribute( "Id" ).Value ),
                    VPackageIds = x.Elements( "Dependency" )
                                    .Select( d => new VPackageId(
                                        d.Attribute( "PackageManager" ).Value,
                                        d.Attribute( "Id" ).Value,
                                        d.Attribute( "Version" ).Value ) )
                } )
                .ToDictionary( x => x.Id, x => x.VPackageIds );
        }
    }
}
