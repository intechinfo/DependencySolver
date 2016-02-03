using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Invenietis.DependencyCrawler.IO
{
    public class AzureTablePackageRepository : IPackageRepository
    {
        readonly string _connectionString;

        public AzureTablePackageRepository( string connectionString )
        {
            _connectionString = connectionString;
        }

        public async Task AddOrUpdatePackage( Package package )
        {
            TableOperation operation = TableOperation.InsertOrReplace( new PackageEntity( package ) );
            await PackageTable.ExecuteAsync( operation );
        }

        public async Task AddOrUpdateVPackage( VPackage vPackage )
        {
            TableOperation operation = TableOperation.InsertOrReplace( new VPackageEntity( vPackage ) );
            await VPackageTable.ExecuteAsync( operation );
        }

        public async Task<bool> ExistsVPackage( VPackageId vPackageId )
        {
            TableOperation operation = TableOperation.Retrieve( "NuGet", $"{vPackageId.Id}_{vPackageId.Version}" );
            TableResult result = await VPackageTable.ExecuteAsync( operation );
            return result.Result != null;
        }

        public async Task<Package> GetPackageById( PackageId packageId )
        {
            TableOperation operation = TableOperation.Retrieve<PackageEntity>( "NuGet", $"{packageId.Id}" );
            TableResult result = await PackageTable.ExecuteAsync( operation );
            if( result.Result == null ) return null;
            return ( ( PackageEntity )result.Result ).ToPackage();
        }

        public async Task<VPackage> GetVPackageById( VPackageId vPackageId )
        {
            TableOperation operation = TableOperation.Retrieve<VPackageEntity>( "NuGet", $"{vPackageId.Id}_{vPackageId.Version}" );
            TableResult result = await VPackageTable.ExecuteAsync( operation );
            if( result.Result == null ) return null;
            return ( ( VPackageEntity )result.Result ).ToVPackage();
        }

        public async Task<IReadOnlyCollection<PackageId>> GetRootPackages()
        {
            TableQuery<RootPackageEntity> query = new TableQuery<RootPackageEntity>()
                .Where( TableQuery.GenerateFilterCondition( "PartitionKey", QueryComparisons.Equal, "NuGet" ) );
            TableContinuationToken continuationToken = null;
            List<PackageId> result = new List<PackageId>();
            do
            {
                TableQuerySegment<RootPackageEntity> segment = await RootPackageTable.ExecuteQuerySegmentedAsync( query, continuationToken );
                continuationToken = segment.ContinuationToken;
                result.AddRange( segment.Results.Select( e => new PackageId( e.RowKey ) ) );
            } while( continuationToken != null );

            return result;
        }

        CloudStorageAccount _storageAccount;
        CloudStorageAccount StorageAccount
        {
            get { return _storageAccount ?? ( _storageAccount = CloudStorageAccount.Parse( _connectionString ) ); }
        }

        CloudTableClient _tableClient;
        CloudTableClient TableClient
        {
            get { return _tableClient ?? ( _tableClient = StorageAccount.CreateCloudTableClient() ); }
        }

        CloudTable _packageTable;
        CloudTable PackageTable
        {
            get { return _packageTable ?? ( _packageTable = TableClient.GetTableReference( "Package" ) ); }
        }

        CloudTable _vPackageTable;
        CloudTable VPackageTable
        {
            get { return _vPackageTable ?? ( _vPackageTable = TableClient.GetTableReference( "VPackage" ) ); }
        }

        CloudTable _rootPackageTable;
        CloudTable RootPackageTable
        {
            get { return _rootPackageTable ?? ( _rootPackageTable = TableClient.GetTableReference( "RootPackage" ) ); }
        }

        public class VPackageEntity : TableEntity
        {
            public VPackageEntity( VPackage vPackage )
                : this()
            {
                PartitionKey = "NuGet";
                RowKey = $"{vPackage.VPackageId.Id}_{vPackage.VPackageId.Version}";
                Value = vPackage.ToXml();
            }

            public VPackageEntity()
            {
            }

            public string Value { get; set; }

            internal VPackage ToVPackage()
            {
                return Value.ToVPackage();
            }
        }

        public class PackageEntity : TableEntity
        {
            public PackageEntity( Package package )
            {
                PartitionKey = "NuGet";
                RowKey = package.Name;
                LastRelease = package.LastRelease.ToXml();
                LastPreRelease = package.LastPreRelease.ToXml();
                IsNotFound = package.IsNotFound;
            }

            public PackageEntity()
            {
            }

            public string LastRelease { get; set; }

            public string LastPreRelease { get; set; }

            public bool IsNotFound { get; set; }

            internal Package ToPackage()
            {
                return new Package( RowKey, LastRelease.ToVPackage(), LastPreRelease.ToVPackage(), IsNotFound );
            }
        }

        class RootPackageEntity : TableEntity
        {
            public RootPackageEntity( PackageId packageId )
            {
                PartitionKey = "NuGet";
                RowKey = packageId.Id;
            }

            public RootPackageEntity()
            {
            }
        }
    }

    internal static class VPackageExtensions
    {
        internal static string ToXml( this VPackage @this )
        {
            if( @this == null ) return string.Empty;
            XDocument xDocument = new XDocument(
                new XElement( "VPackage",
                    new XAttribute( "Id", @this.VPackageId.Id ),
                    new XAttribute( "Version", @this.VPackageId.Version ),
                    new XAttribute( "IsNotFound", @this.IsNotFound ),
                    new XElement(
                        "Dependencies",
                        @this.Dependencies.Select(
                            d => new XElement(
                                "Dependency",
                                new XAttribute( "Id", d.Id ),
                                new XAttribute( "Version", d.Version ) ) ) ) ) );
            return xDocument.ToString();
        }
    }

    internal static class StringExtensions
    {
        internal static VPackage ToVPackage( this string @this )
        {
            if( @this == null || @this == string.Empty ) return null;
            XElement xElement = XElement.Parse( @this );
            VPackageId vPackageId = new VPackageId( xElement.Attribute( "Id" ).Value, xElement.Attribute( "Version" ).Value );
            bool isNotFound = bool.Parse( xElement.Attribute( "IsNotFound" ).Value );
            IReadOnlyCollection<VPackageId> dependencies = xElement.Element( "Dependencies" )
                .Elements()
                .Select( x => new VPackageId(
                     x.Attribute( "Id" ).Value,
                     x.Attribute( "Version" ).Value ) )
                .ToList();
            return new VPackage( vPackageId, dependencies, isNotFound );
        }
    }
}
