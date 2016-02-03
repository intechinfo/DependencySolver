using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.IO;
using Microsoft.Extensions.Configuration;

namespace Invenietis.DependencyCrawler.Hosts.Conductor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile( "appsettings.json" )
                .Build();

            string connectionString = config[ "Data:DefaultConnection:AzureStorage" ];
            CrawlingConductor conductor = new CrawlingConductor(
                new AzureJobQueue( connectionString, "conductorqueue" ),
                new AzureTablePackageRepository( connectionString ) );

            conductor.AddOutQueue(
                new AzureJobQueue( connectionString, "atozjobs" ) );

            Task.Run( async () => await conductor.Start() ).Wait();
        }
    }
}
