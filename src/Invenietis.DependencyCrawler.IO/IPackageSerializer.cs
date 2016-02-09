using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.IO
{
    public interface IPackageSerializer
    {
        string Serialize( VPackage vPackage );

        VPackage DeserializeVPackage( string serializedVPackage );
    }
}