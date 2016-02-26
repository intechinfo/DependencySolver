using Invenietis.DependencyCrawler.Core;

namespace DependencyManager.Models
{
    public class PackageLastVersion
    {
        public string Id { get; set; }

        public string PackageManager { get; set; }

        public CombinedVPackageId LastVersions { get; set; }
    }
}