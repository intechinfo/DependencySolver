using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IJobVisitor
    {
        Task Visit( CrawlPackageJob job );

        Task Visit( CrawlVPackageJob job );

        Task Visit( VPackageCrawledJob job );

        Task Visit( PackageCrawledJob job );

        Task Visit( StopJob stopJob );
    }
}