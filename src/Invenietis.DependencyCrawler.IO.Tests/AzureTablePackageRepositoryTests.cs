using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Abstractions.Tests;
using Invenietis.DependencyCrawler.IO;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.Tests
{
    [TestFixture]
    public class AzureTablePackageRepositoryTests : PackageRepositoryTestsBase
    {
        protected override IPackageRepository CreatePackageRepository()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile( "appsettings.json" )
                .Build();
            string connectionString = configuration[ "Data:AzureStorage:ConnectionString" ];
            string packageTable = configuration[ "Data:AzureStorage:PackageTable" ];
            string vPackageTable = configuration[ "Data:AzureStorage:VPackageTable" ];
            string notCrawledVPackageTable = configuration[ "Data:AzureStorage:NotCrawledVPackageTable" ];
            string vPackageCacheBlobContainer = configuration[ "Data:AzureStorage:VPackageCacheBlobContainer" ];
            return new AzureTablePackageRepository( connectionString, packageTable, vPackageTable, notCrawledVPackageTable, vPackageCacheBlobContainer );
        }
    }
}
