using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler
{
    public class CrawlingConductor : ICrawlingConductor
    {
        readonly HashSet<IOutJobQueue> _outQueues;

        public CrawlingConductor( IInJobQueue queue, IPackageRepository packageRepository )
        {
            Queue = queue;
            PackageRepository = packageRepository;
            _outQueues = new HashSet<IOutJobQueue>();
        }

        public IPackageRepository PackageRepository { get; }

        public IInJobQueue Queue { get; }

        public void AddOutQueue( IOutJobQueue queue )
        {
            _outQueues.Add( queue );
        }

        public async Task Start()
        {
            StopVisitor stopVisitor = new StopVisitor();
            foreach( PackageId p in await PackageRepository.GetRootPackages() ) await Dispatch( new CrawlPackageJob( p ) );
            for( ;;)
            {
                IJob job = await Queue.PeekNextJob();
                if( await stopVisitor.MustStop( job ) )
                {
                    await Dispatch( job );
                    await Queue.TakeNextJob();
                    return;
                }
                await Dispatch( job );
                await Queue.TakeNextJob();
            }
        }

        async Task Dispatch( IJob job )
        {
            foreach( IOutJobQueue outQueue in _outQueues ) await outQueue.PutJob( job );
        }
    }
}
