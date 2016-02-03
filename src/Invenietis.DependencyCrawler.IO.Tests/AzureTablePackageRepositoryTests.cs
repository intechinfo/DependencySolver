using Invenietis.DependencyCrawler.Abstractions.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.IO;
using Microsoft.Extensions.Configuration;

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
            return new AzureTablePackageRepository( configuration[ "Data:AzureTableConnection" ] );
        }
    }
}
