using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Abstractions.Tests
{
    [TestFixture]
    public abstract class JobQueueTestsBase
    {
        [Test]
        public async Task PeekAndTake_WithAllJobTypes_WorksCorrectly()
        {
            IJobQueue sut = CreateJobQueue();
            CrawlPackageJob job1 = new CrawlPackageJob( new PackageId( "P1" ) );
            CrawlVPackageJob job2 = new CrawlVPackageJob( new VPackageId( "P2", "2.0.0" ) );
            PackageCrawledJob job3 = new PackageCrawledJob( new PackageId( "P3" ) );
            StopJob job4 = new StopJob();
            VPackageCrawledJob job5 = new VPackageCrawledJob( new VPackageId( "P4", "4.0.0" ) );

            await sut.Clear();
            await sut.PutJob( job1 );
            await sut.PutJob( job2 );
            await sut.PutJob( job3 );
            await sut.PutJob( job4 );
            await sut.PutJob( job5 );

            CrawlPackageJob peekedResult1 = await sut.PeekNextJob() as CrawlPackageJob;
            CrawlPackageJob takenResult1 = await sut.TakeNextJob() as CrawlPackageJob;
            CrawlVPackageJob peekedResult2 = await sut.PeekNextJob() as CrawlVPackageJob;
            CrawlVPackageJob takenResult2 = await sut.TakeNextJob() as CrawlVPackageJob;
            PackageCrawledJob peekedResult3 = await sut.PeekNextJob() as PackageCrawledJob;
            PackageCrawledJob takenResult3 = await sut.TakeNextJob() as PackageCrawledJob;
            StopJob peekedResult4 = await sut.PeekNextJob() as StopJob;
            StopJob takenResult4 = await sut.TakeNextJob() as StopJob;
            VPackageCrawledJob peekedResult5 = await sut.PeekNextJob() as VPackageCrawledJob;
            VPackageCrawledJob takenResult5 = await sut.TakeNextJob() as VPackageCrawledJob;

            Assert.That( peekedResult1, Is.Not.Null );
            Assert.That( takenResult1, Is.Not.Null );
            Assert.That( peekedResult1.PackageId, Is.EqualTo( new PackageId( "P1" ) ) );
            Assert.That( takenResult1.PackageId, Is.EqualTo( peekedResult1.PackageId ) );

            Assert.That( peekedResult2, Is.Not.Null );
            Assert.That( peekedResult2.VPackageId, Is.EqualTo( new VPackageId( "P2", "2.0.0" ) ) );
            Assert.That( takenResult2, Is.Not.Null );
            Assert.That( takenResult2.VPackageId, Is.EqualTo( peekedResult2.VPackageId ) );

            Assert.That( peekedResult3, Is.Not.Null );
            Assert.That( peekedResult3.PackageId, Is.EqualTo( new PackageId( "P3" ) ) );
            Assert.That( takenResult3, Is.Not.Null );
            Assert.That( takenResult3.PackageId, Is.EqualTo( peekedResult3.PackageId ) );

            Assert.That( peekedResult4, Is.Not.Null );
            Assert.That( takenResult4, Is.Not.Null );

            Assert.That( peekedResult5, Is.Not.Null );
            Assert.That( peekedResult5.VPackageId, Is.EqualTo( new VPackageId( "P4", "4.0.0" ) ) );
            Assert.That( takenResult5, Is.Not.Null );
            Assert.That( takenResult5.VPackageId, Is.EqualTo( peekedResult5.VPackageId ) );
        }

        protected abstract IJobQueue CreateJobQueue();
    }
}
