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
        readonly string _tableName;

        public AzureTablePackageRepository( string connectionString, string tableName )
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public async Task AddDependenciesIfNotExists( VPackageId vPackageId, IEnumerable<VPackageId> dependencies )
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<NotCrawledVPackageEntity>( vPackageId.PackageManager, $"NotCrawledVPackage|{vPackageId.Id}|{vPackageId.Version}" );
            TableResult result = await PackageTable.ExecuteAsync( retrieveOperation );
            NotCrawledVPackageEntity notCrawledEntity = ( NotCrawledVPackageEntity )result.Result;
            if( notCrawledEntity == null ) return;

            TableOperation deleteOperation = TableOperation.Delete( notCrawledEntity );
            await PackageTable.ExecuteAsync( deleteOperation );
            retrieveOperation = TableOperation.Retrieve<VPackageEntity>( vPackageId.PackageManager, $"VPackage|{vPackageId.Id}|{vPackageId.Version}" );
            result = await PackageTable.ExecuteAsync( retrieveOperation );
            VPackageEntity vPackageEntity = ( VPackageEntity )result.Result;
            if( vPackageEntity == null ) return;

            vPackageEntity.Dependencies = SerializeDependencies( dependencies );
            TableOperation insertOperation = TableOperation.InsertOrReplace( vPackageEntity );
            await PackageTable.ExecuteAsync( insertOperation );
        }

        string SerializeDependencies( IEnumerable<VPackageId> dependencies )
        {
            if( dependencies == null ) dependencies = new VPackageId[ 0 ];
            return new XElement(
                "Dependencies",
                dependencies.Select( d => new XElement(
                    "Dependency",
                    new XAttribute( "PackageManager", d.PackageManager ),
                    new XAttribute( "Id", d.Id ),
                    new XAttribute( "Version", d.Version ) ) ) )
                .ToString();
        }

        public Task<bool> AddIfNotExists( PackageId packageId )
        {
            return AddIfNotExists(
                new PackageEntity( packageId ),
                packageId.PackageManager,
                $"Package|{packageId.Value}" );
        }

        public async Task<bool> AddIfNotExists( VPackageId vPackageId )
        {
            TableOperation retrieveOperation = TableOperation.Retrieve( vPackageId.PackageManager, $"VPackage|{vPackageId.Id}|{vPackageId.Version}" );
            TableResult retrieved = await PackageTable.ExecuteAsync( retrieveOperation );
            if( retrieved.Result != null ) return false;

            TableOperation insertOperation = TableOperation.Insert( new NotCrawledVPackageEntity( vPackageId ) );
            await PackageTable.ExecuteAsync( insertOperation );
            insertOperation = TableOperation.Insert( new VPackageEntity( vPackageId ) );
            await PackageTable.ExecuteAsync( insertOperation );
            return true;
        }

        async Task<bool> AddIfNotExists( ITableEntity entity, string partitionKey, string rowKey )
        {
            TableOperation retrieveOperation = TableOperation.Retrieve( partitionKey, rowKey );
            TableResult retrieved = await PackageTable.ExecuteAsync( retrieveOperation );
            if( retrieved.Result != null ) return false;

            TableOperation insertOperation = TableOperation.Insert( entity );
            await PackageTable.ExecuteAsync( insertOperation );
            return true;
        }

        public async Task<IEnumerable<Package>> GetAllPackages()
        {
            string filter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition( "RowKey", QueryComparisons.GreaterThanOrEqual, $"Package|" ),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition( "RowKey", QueryComparisons.LessThan, $"Package||" ) );

            TableQuery<PackageEntity> query = new TableQuery<PackageEntity>().Where( filter );
            TableContinuationToken continuationToken = null;
            List<Package> result = new List<Package>();
            do
            {
                TableQuerySegment<PackageEntity> s = await PackageTable.ExecuteQuerySegmentedAsync( query, continuationToken );
                continuationToken = s.ContinuationToken;
                foreach( PackageEntity e in s.Results ) result.Add( await PackageEntityToPackage( e ) );
            } while( continuationToken != null );
            return result;
        }

        public Task<IEnumerable<VPackageId>> GetNotCrawledVPackageIds( PackageSegment segment )
        {
            return GetIds<VPackageEntity, VPackageId>(
                segment,
                "NotCrawledVPackage",
                i =>
                {
                    string[] keyParts = i.RowKey.Split( '|' );
                    return new VPackageId( segment.PackageManager, keyParts[ 1 ], keyParts[ 2 ] );
                } );
        }

        public Task<IEnumerable<PackageId>> GetPackageIds( PackageSegment segment )
        {
            return GetIds<PackageEntity, PackageId>(
                segment,
                "Package",
                i =>
                {
                    string[] keyParts = i.RowKey.Split( '|' );
                    return new PackageId( segment.PackageManager, keyParts[ 1 ] );
                } );
        }

        public Task<IEnumerable<VPackageId>> GetVPackageIds( PackageSegment segment )
        {
            return GetIds<VPackageEntity, VPackageId>(
                segment,
                "VPackage",
                i =>
                {
                    string[] keyParts = i.RowKey.Split( '|' );
                    return new VPackageId( segment.PackageManager, keyParts[ 1 ], keyParts[ 2 ] );
                } );
        }

        async Task<IEnumerable<TResult>> GetIds<T, TResult>( PackageSegment segment, string rowKeyPrefix, Func<T, TResult> map )
            where T : ITableEntity, new()
        {
            string filter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition( "PartitionKey", QueryComparisons.Equal, segment.PackageManager ),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition( "RowKey", QueryComparisons.GreaterThanOrEqual, $"{rowKeyPrefix}|{segment.Start}" ) );

            if( segment.HasEnd )
            {
                filter = TableQuery.CombineFilters(
                    filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition( "RowKey", QueryComparisons.LessThan, $"{rowKeyPrefix}|{segment.End}" ) );
            }
            else
            {
                filter = TableQuery.CombineFilters(
                    filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition( "RowKey", QueryComparisons.LessThan, $"{rowKeyPrefix}||" ) );
            }
            TableQuery<T> query = new TableQuery<T>().Where( filter );
            TableContinuationToken continuationToken = null;
            List<TResult> result = new List<TResult>();
            do
            {
                TableQuerySegment<T> s = await PackageTable.ExecuteQuerySegmentedAsync( query, continuationToken );
                continuationToken = s.ContinuationToken;
                result.AddRange( s.Results.Select( r => map( r ) ) );
            } while( continuationToken != null );
            return result;
        }

        public Task UpdateLastPreRelease( PackageId id, VPackageId lastPreReleaseId )
        {
            return UpdateLastRelease( id, e => e.LastPreRelease = lastPreReleaseId.Version );
        }

        public Task UpdateLastRelease( PackageId id, VPackageId lastReleaseId )
        {
            return UpdateLastRelease( id, e => e.LastRelease = lastReleaseId.Version );
        }

        async Task UpdateLastRelease( PackageId id, Action<PackageEntity> update )
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<PackageEntity>( id.PackageManager, $"Package|{id.Value}" );
            TableResult tableResult = await PackageTable.ExecuteAsync( retrieveOperation );
            PackageEntity entity = ( PackageEntity )tableResult.Result;
            if( entity != null )
            {
                update( entity );
                TableOperation updateOperation = TableOperation.Replace( entity );
                await PackageTable.ExecuteAsync( updateOperation );
            }
        }

        public async Task<Package> GetPackageById( PackageId packageId )
        {
            TableOperation retrieve = TableOperation.Retrieve<PackageEntity>( packageId.PackageManager, $"Package|{packageId.Value}" );
            TableResult tableResult = await PackageTable.ExecuteAsync( retrieve );
            PackageEntity entity = ( PackageEntity )tableResult.Result;
            if( entity == null ) return null;

            return await PackageEntityToPackage( entity );
        }

        async Task<Package> PackageEntityToPackage( PackageEntity entity )
        {
            string[] keyParts = entity.RowKey.Split( '|' );
            VPackage lastRelease = string.IsNullOrWhiteSpace( entity.LastRelease ) ? null : await GetVPackageById( new VPackageId( entity.PartitionKey, keyParts[ 1 ], entity.LastRelease ) );
            VPackage lastPreRelease = string.IsNullOrWhiteSpace( entity.LastPreRelease ) ? null : await GetVPackageById( new VPackageId( entity.PartitionKey, keyParts[ 1 ], entity.LastPreRelease ) );
            return new Package( new PackageId( entity.PartitionKey, keyParts[ 1 ] ), lastRelease, lastPreRelease );
        }

        async Task<VPackage> GetVPackageById( VPackageId vPackageId )
        {
            TableOperation retrieve = TableOperation.Retrieve<VPackageEntity>( vPackageId.PackageManager, $"VPackage|{vPackageId.Id}|{vPackageId.Version}" );
            TableResult tableResult = await PackageTable.ExecuteAsync( retrieve );
            VPackageEntity entity = ( VPackageEntity )tableResult.Result;
            if( entity == null ) return null;

            if( string.IsNullOrWhiteSpace( entity.Dependencies ) ) return new VPackage( vPackageId );

            List<VPackage> dependencies = new List<VPackage>();
            foreach( VPackageId dependency in Deserialize( entity.Dependencies ) )
            {
                VPackage vPackage = await GetVPackageById( dependency );
                if( vPackage != null ) dependencies.Add( vPackage );
            }

            return new VPackage( vPackageId, dependencies );
        }

        CloudStorageAccount _cloudStorageAccount;
        CloudStorageAccount CloudStorageAccount
        {
            get { return _cloudStorageAccount ?? ( _cloudStorageAccount = CloudStorageAccount.Parse( _connectionString ) ); }
        }

        CloudTableClient _cloudTableClient;
        CloudTableClient CloudTableClient
        {
            get { return _cloudTableClient ?? ( _cloudTableClient = CloudStorageAccount.CreateCloudTableClient() ); }
        }

        CloudTable _packageTable;
        CloudTable PackageTable
        {
            get { return _packageTable ?? ( _packageTable = CloudTableClient.GetTableReference( _tableName ) ); }
        }

        static IReadOnlyCollection<VPackageId> Deserialize( string dependencies )
        {
            return XElement.Parse( dependencies )
                .Descendants()
                .Select( d => new VPackageId(
                     d.Attribute( "PackageManager" ).Value,
                     d.Attribute( "Id" ).Value,
                     d.Attribute( "Version" ).Value ) )
                .ToList();
        }

        public class PackageEntity : TableEntity
        {
            public PackageEntity()
            {
            }

            public PackageEntity( PackageId packageId )
            {
                PartitionKey = packageId.PackageManager;
                RowKey = $"Package|{packageId.Value}";
            }

            public string LastRelease { get; set; }

            public string LastPreRelease { get; set; }
        }

        public class VPackageEntity : TableEntity
        {
            public VPackageEntity()
            {
            }

            public VPackageEntity( VPackageId vPackageId )
            {
                PartitionKey = vPackageId.PackageManager;
                RowKey = $"VPackage|{vPackageId.Id}|{vPackageId.Version}";
            }

            public string Dependencies { get; set; }
        }

        public class NotCrawledVPackageEntity : TableEntity
        {
            public NotCrawledVPackageEntity()
            {
            }

            public NotCrawledVPackageEntity( VPackageId vPackageId )
            {
                PartitionKey = vPackageId.PackageManager;
                RowKey = $"NotCrawledVPackage|{vPackageId.Id}|{vPackageId.Version}";
            }
        }
    }
}
