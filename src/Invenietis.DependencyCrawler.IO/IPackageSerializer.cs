using System.Collections.Generic;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.IO
{
    public interface IPackageSerializer
    {
        string Serialize( VPackage vPackage );

        VPackage DeserializeVPackage( string serializedVPackage );

        string Serialize( IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>> dependencies );

        IReadOnlyDictionary<PlatformId, IEnumerable<VPackageId>> DeserializeVPackageDependencies( string serializedVPackageDependencies );
    }
}