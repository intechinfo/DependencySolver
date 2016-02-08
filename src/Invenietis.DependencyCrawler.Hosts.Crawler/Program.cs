﻿using System;
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
        public static void Main( string[] args )
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile( "appsettings.json" )
                .Build();

            string connectionString = config[ "Data:AzureStorage:ConnectionString" ];
            string packageTable = config[ "Data:AzureStorage:PackageTable" ];
            string vPackageTable = config[ "Data:AzureStorage:VPackageTable" ];
            string notCrawledVPackageTable = config[ "Data:AzureStorage:NotCrawledVPackageTable" ];
            string vPackageCacheBlobContainer = config[ "Data:AzureStorage:VPackageCacheBlobContainer" ];
            Crawler crawler = new Crawler(
                new NuGetDownloader(
                    new FeedProvider( new[] { "http://nuget.org/api/v2/" } ) ),
                new AzureTablePackageRepository( connectionString, packageTable, vPackageTable, notCrawledVPackageTable, vPackageCacheBlobContainer ),
                new PackageSegment( PackageId.NuGet, "A" ) );

            Task.Run( async () => await crawler.Start() ).Wait();
        }
    }
}
