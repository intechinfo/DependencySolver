using Invenietis.DependencyCrawler.Core;
using System.Threading.Tasks;
using System;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IJob
    {
        Task Accept( IJobVisitor visitor );
    }

    public class CrawlPackageJob : IJob
    {
        public CrawlPackageJob( PackageId packageId )
        {
            PackageId = packageId;
        }

        public Task Accept( IJobVisitor visitor )
        {
            return visitor.Visit( this );
        }

        public PackageId PackageId { get; }
    }

    public class CrawlVPackageJob : IJob
    {
        public CrawlVPackageJob( VPackageId vPackageId )
        {
            VPackageId = vPackageId;
        }

        public Task Accept( IJobVisitor visitor )
        {
            return visitor.Visit( this );
        }

        public VPackageId VPackageId { get; }
    }

    public class VPackageCrawledJob : IJob
    {
        public VPackageCrawledJob( VPackageId vPackageId )
        {
            VPackageId = vPackageId;
        }

        public Task Accept( IJobVisitor visitor )
        {
            return visitor.Visit( this );
        }

        public VPackageId VPackageId { get; }
    }

    public class PackageCrawledJob : IJob
    {
        public PackageCrawledJob( PackageId packageId )
        {
            PackageId = packageId;
        }

        public Task Accept( IJobVisitor visitor )
        {
            return visitor.Visit( this );
        }

        public PackageId PackageId { get; }
    }

    public class StopJob : IJob
    {
        public Task Accept( IJobVisitor visitor )
        {
            return visitor.Visit( this );
        }
    }
}