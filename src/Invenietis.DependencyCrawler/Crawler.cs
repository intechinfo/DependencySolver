using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler
{
    public class Crawler : ICrawler
    {
        public Crawler(
            IPackageDownloader downloader,
            IPackageRepository repository,
            ICrawlerStateStorage stateStorage,
            IInJobQueue inJobQueue,
            IOutJobQueue outJobQueue,
            PackageSegment segment )
        {
            PackageDownloader = downloader;
            PackageRepository = repository;
            CrawlerStateStorage = stateStorage;
            InJobQueue = inJobQueue;
            OutJobQueue = outJobQueue;
            Segment = segment;
        }

        public ICrawlerStateStorage CrawlerStateStorage { get; }

        public IInJobQueue InJobQueue { get; }

        public IOutJobQueue OutJobQueue { get; }

        public IPackageDownloader PackageDownloader { get; }

        public IPackageRepository PackageRepository { get; }

        public PackageSegment Segment { get; }

        public async Task Start()
        {
            JobProcessor processor = new JobProcessor( PackageDownloader, OutJobQueue, PackageRepository );
            StopVisitor stopVisitor = new StopVisitor();
            for( ;; )
            {
                IJob job = await InJobQueue.PeekNextJob();
                if( await stopVisitor.MustStop( job ) )
                {
                    await InJobQueue.TakeNextJob();
                    return;
                }
                await processor.Execute( job );
                await InJobQueue.TakeNextJob();
            }
        }
    }
}
