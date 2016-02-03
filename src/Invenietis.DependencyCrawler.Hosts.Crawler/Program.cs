using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;
using Invenietis.DependencyCrawler.IO;
using Microsoft.Extensions.Configuration;

namespace Invenietis.DependencyCrawler.Hosts
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile( "appsettings.json" )
                .Build();

            string connectionString = config[ "Data:DefaultConnection:AzureStorage" ];
            Crawler crawler = new Crawler(
                new NuGetDownloader(
                    new FeedProvider( new[] { "http://nuget.org/api/v2/" } ) ),
                new AzureTablePackageRepository( connectionString ),
                null,
                new AzureJobQueue( connectionString, "atozjobs" ),
                new AzureJobQueue( connectionString, "conductorqueue" ),
                new PackageSegment( "a", "z" ) );

            Task.Run( async () => await crawler.Start() ).Wait();
        }
    }
}
