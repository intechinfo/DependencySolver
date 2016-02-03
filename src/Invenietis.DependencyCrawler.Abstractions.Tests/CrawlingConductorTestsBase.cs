using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;
using NSubstitute;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    [TestFixture]
    public abstract class CrawlingConductorTestsBase
    {
        [Test]
        public async Task Start_ShouldDispatchJobsToTheRightQueues()
        {
            IPackageRepository packageRepository = Substitute.For<IPackageRepository>();
            IInJobQueue conductorQueue = Substitute.For<IInJobQueue>();
            ICrawlingConductor sut = CreateCrawlingConductor( conductorQueue, packageRepository );
            IOutJobQueue queue1 = Substitute.For<IOutJobQueue>();
            IOutJobQueue queue2 = Substitute.For<IOutJobQueue>();
            IOutJobQueue queue3 = Substitute.For<IOutJobQueue>();

            sut.AddOutQueue( queue1 );
            sut.AddOutQueue( queue2 );
            sut.AddOutQueue( queue3 );

            conductorQueue.PeekNextJob().Returns(
                new CrawlPackageJob( new PackageId( "Package1" ) ),
                new CrawlVPackageJob( new VPackageId( "Package1", "1.0.0" ) ),
                new StopJob() );
            conductorQueue.TakeNextJob().Returns( new CrawlPackageJob( new PackageId( "Package1" ) ),
                new CrawlVPackageJob( new VPackageId( "Package1", "1.0.0" ) ),
                new StopJob() );

            List<IJob> received = new List<IJob>();
            queue2.When( q => q.PutJob( Arg.Any<IJob>() ) ).Do( i => received.Add( i.ArgAt<IJob>( 0 ) ) );

            await sut.Start();

            await queue1.Received( 3 ).PutJob( Arg.Any<IJob>() );
            await queue3.Received( 3 ).PutJob( Arg.Any<IJob>() );
            await queue2.Received( 3 ).PutJob( Arg.Any<IJob>() );
        }

        [Test]
        public async Task Start_ShouldStartRootPackagesCrawling()
        {
            FakePackageRepository packageRepository = new FakePackageRepository();
            packageRepository.SetRootPackages( new[] { new PackageId( "Package1" ), new PackageId( "Package2" ) } );
            IReadOnlyCollection<PackageId> rootPackageIds = await packageRepository.GetRootPackages();
            IInJobQueue conductorQueue = Substitute.For<IInJobQueue>();
            ICrawlingConductor sut = CreateCrawlingConductor( conductorQueue, packageRepository );
            IOutJobQueue outQueue = Substitute.For<IOutJobQueue>();

            sut.AddOutQueue( outQueue );

            conductorQueue.PeekNextJob().Returns( new StopJob() );
            List<IJob> received = new List<IJob>();
            outQueue.When( q => q.PutJob( Arg.Any<IJob>() ) ).Do( i => received.Add( i.ArgAt<IJob>( 0 ) ) );

            await sut.Start();

            await outQueue.Received( 3 ).PutJob( Arg.Any<IJob>() );
            CrawlPackageJob job1 = received[ 0 ] as CrawlPackageJob;
            CrawlPackageJob job2 = received[ 1 ] as CrawlPackageJob;
            Assert.That( job1, Is.Not.Null );
            Assert.That( job1.PackageId, Is.EqualTo( new PackageId( "Package1" ) ) );
            Assert.That( job2, Is.Not.Null );
            Assert.That( job2.PackageId, Is.EqualTo( new PackageId( "Package2" ) ) );
        }

        protected abstract ICrawlingConductor CreateCrawlingConductor( IInJobQueue queue, IPackageRepository packageRepository );
    }
}
