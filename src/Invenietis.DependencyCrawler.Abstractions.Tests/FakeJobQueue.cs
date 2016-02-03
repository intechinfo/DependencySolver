using System.Collections.Generic;
using System.Threading.Tasks;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    class FakeJobQueue : IInJobQueue, IOutJobQueue
    {
        readonly Queue<IJob> _jobs;

        internal FakeJobQueue()
        {
            _jobs = new Queue<IJob>();
        }

#pragma warning disable 1998

        public async Task<IJob> PeekNextJob()
        {
            if( _jobs.Count != 0 ) return _jobs.Peek();
            return new StopJob();
        }

        public async Task PutJob( IJob job )
        {
            _jobs.Enqueue( job );
        }

        public async Task<IJob> TakeNextJob()
        {
            if( _jobs.Count != 0 ) return _jobs.Dequeue();
            return new StopJob();
        }

#pragma warning restore 1998

    }
}
