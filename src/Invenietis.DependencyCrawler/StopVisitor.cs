using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;

namespace Invenietis.DependencyCrawler
{
    public class StopVisitor : IJobVisitor
    {
        bool _mustStop;

#pragma warning disable 1998

        public async Task Visit( VPackageCrawledJob job )
        {
            _mustStop = false;
        }

        public async Task Visit( StopJob stopJob )
        {
            _mustStop = true;
        }

        public async Task Visit( PackageCrawledJob job )
        {
            _mustStop = false;
        }

        public async Task Visit( CrawlVPackageJob job )
        {
            _mustStop = false;
        }

        public async Task Visit( CrawlPackageJob job )
        {
            _mustStop = false;
        }

#pragma warning restore 1998

        public async Task<bool> MustStop( IJob job )
        {
            await job.Accept( this );
            return _mustStop;
        }
    }
}
