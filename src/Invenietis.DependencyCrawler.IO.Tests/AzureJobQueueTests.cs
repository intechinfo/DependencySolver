using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Abstractions.Tests;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Invenietis.DependencyCrawler.IO.Tests
{
    [TestFixture]
    public class AzureJobQueueTests : JobQueueTestsBase
    {
        protected override IJobQueue CreateJobQueue()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile( "appsettings.json" )
                .Build();
            return new AzureJobQueue( configuration[ "Data:AzureQueueConnection" ], configuration[ "Data:AzureQueueName" ] );
        }
    }
}
