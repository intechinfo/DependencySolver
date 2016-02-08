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
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Invenietis.DependencyCrawler.IO
{
    public class AzureTablePackageRepository : IPackageRepository
    {
        readonly string _connectionString;
        readonly string _packageTableName;
        readonly string _vPackageTableName;
        readonly string _notCrawledTableName;
        readonly string _vPackageCacheBlobContainerName;
        readonly TimeSpan _cacheTimeOut;

        public AzureTablePackageRepository(
            string connectionString,
            string packageTableName,
            string vPackageTableName,
            string notCrawledTableName,
            string vPackageCacheBlobContainerName )
            : this(
                  connectionString,
                  packageTableName,
                  vPackageTableName,
                  notCrawledTableName,
                  vPackageCacheBlobContainerName,
                  TimeSpan.FromMinutes( 1 ) )
        {
        }

        public AzureTablePackageRepository(
            string connectionString,
            string packageTableName,
            string vPackageTableName,
            string notCrawledTableName,
            string vPackageCacheBlobContainerName,
            TimeSpan cacheTimeOut )
        {
            _connectionString = connectionString;
            _packageTableName = packageTableName;
            _vPackageTableName = vPackageTableName;
            _notCrawledTableName = notCrawledTableName;
            _vPackageCacheBlobContainerName = vPackageCacheBlobContainerName;
            _cacheTimeOut = cacheTimeOut;
        }

        public async Task AddDependenciesIfNotExists( VPackageId vPackageId, IEnumerable<VPackageId> dependencies )
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<NotCrawledVPackageEntity>( vPackageId.PackageManager, $"{vPackageId.Id}|{vPackageId.Version}" );
            TableResult result = await NotCrawledVPackageTable.ExecuteAsync( retrieveOperation );
            NotCrawledVPackageEntity notCrawledEntity = ( NotCrawledVPackageEntity )result.Result;
            if( notCrawledEntity == null ) return;

            VPackageEntity vPackageEntity = new VPackageEntity( vPackageId );
            vPackageEntity.Dependencies = SerializeDependencies( dependencies );
            TableOperation insertOperation = TableOperation.InsertOrReplace( vPackageEntity );
            await VPackageTable.ExecuteAsync( insertOperation );

            TableOperation deleteOperation = TableOperation.Delete( notCrawledEntity );
            await NotCrawledVPackageTable.ExecuteAsync( deleteOperation );
        }

        public async Task<bool> AddIfNotExists( PackageId packageId )
        {
            TableOperation retrieveOperation = TableOperation.Retrieve( packageId.PackageManager, packageId.Value );
            TableResult retrieved = await PackageTable.ExecuteAsync( retrieveOperation );
            if( retrieved.Result != null ) return false;

            TableOperation insertOperation = TableOperation.Insert( new PackageEntity( packageId ) );
            await PackageTable.ExecuteAsync( insertOperation );
            return true;
        }

        public async Task<bool> AddIfNotExists( VPackageId vPackageId )
        {
            TableOperation retrieveOperation = TableOperation.Retrieve( vPackageId.PackageManager, $"{vPackageId.Id}|{vPackageId.Version}" );
            TableResult vPackageRetrieved = await VPackageTable.ExecuteAsync( retrieveOperation );
            TableResult notCrawledVpackageRetrieved = await NotCrawledVPackageTable.ExecuteAsync( retrieveOperation );
            if( vPackageRetrieved.Result != null || notCrawledVpackageRetrieved.Result != null ) return false;

            TableOperation insertOperation = TableOperation.Insert( new NotCrawledVPackageEntity( vPackageId ) );
            await NotCrawledVPackageTable.ExecuteAsync( insertOperation );
            return true;
        }

        public async Task<IEnumerable<Package>> GetAllPackages()
        {
            TableQuery<PackageEntity> query = new TableQuery<PackageEntity>();
            TableContinuationToken continuationToken = null;
            List<Package> result = new List<Package>();
            do
            {
                TableQuerySegment<PackageEntity> s = await PackageTable.ExecuteQuerySegmentedAsync( query, continuationToken );
                continuationToken = s.ContinuationToken;
                foreach( PackageEntity e in s.Results ) result.Add( await GetPackage( e ) );
            } while( continuationToken != null );
            return result;
        }

        public Task<IEnumerable<VPackageId>> GetNotCrawledVPackageIds( PackageSegment segment )
        {
            return GetIds<VPackageEntity, VPackageId>(
                segment,
                NotCrawledVPackageTable,
                i =>
                {
                    string[] keyParts = i.RowKey.Split( '|' );
                    return new VPackageId( segment.PackageManager, keyParts[ 0 ], keyParts[ 1 ] );
                } );
        }

        public Task<IEnumerable<PackageId>> GetPackageIds( PackageSegment segment )
        {
            return GetIds<PackageEntity, PackageId>(
                segment,
                PackageTable,
                i => new PackageId( segment.PackageManager, i.RowKey ) );
        }

        public Task<IEnumerable<VPackageId>> GetVPackageIds( PackageSegment segment )
        {
            return GetIds<VPackageEntity, VPackageId>(
                segment,
                VPackageTable,
                i =>
                {
                    string[] keyParts = i.RowKey.Split( '|' );
                    return new VPackageId( segment.PackageManager, keyParts[ 0 ], keyParts[ 1 ] );
                } );
        }

        async Task<IEnumerable<TResult>> GetIds<T, TResult>( PackageSegment segment, CloudTable table, Func<T, TResult> map )
            where T : ITableEntity, new()
        {
            string filter = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition( "PartitionKey", QueryComparisons.Equal, segment.PackageManager ),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition( "RowKey", QueryComparisons.GreaterThanOrEqual, $"{segment.Start}" ) );

            if( segment.HasEnd )
            {
                filter = TableQuery.CombineFilters(
                    filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition( "RowKey", QueryComparisons.LessThan, $"{segment.End}" ) );
            }

            TableQuery<T> query = new TableQuery<T>().Where( filter );
            TableContinuationToken continuationToken = null;
            List<TResult> result = new List<TResult>();
            do
            {
                TableQuerySegment<T> s = await table.ExecuteQuerySegmentedAsync( query, continuationToken );
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
            TableOperation retrieveOperation = TableOperation.Retrieve<PackageEntity>( id.PackageManager, id.Value );
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
            TableOperation retrieve = TableOperation.Retrieve<PackageEntity>( packageId.PackageManager, packageId.Value );
            TableResult tableResult = await PackageTable.ExecuteAsync( retrieve );
            PackageEntity entity = ( PackageEntity )tableResult.Result;
            if( entity == null ) return null;

            return await GetPackage( entity );
        }

        async Task<Package> GetPackage( PackageEntity entity )
        {
            VPackage lastRelease = string.IsNullOrWhiteSpace( entity.LastRelease ) ? null : await GetVPackage( new VPackageId( entity.PartitionKey, entity.RowKey, entity.LastRelease ) );
            VPackage lastPreRelease = string.IsNullOrWhiteSpace( entity.LastPreRelease ) ? null : await GetVPackage( new VPackageId( entity.PartitionKey, entity.RowKey, entity.LastPreRelease ) );
            return new Package( new PackageId( entity.PartitionKey, entity.RowKey ), lastRelease, lastPreRelease );
        }

        async Task<VPackage> GetVPackage( VPackageId vPackageId )
        {
            var cachedVPackage = await GetCachedVPackage( vPackageId );
            if( cachedVPackage.Item1 ) return cachedVPackage.Item2;

            TableOperation retrieve = TableOperation.Retrieve<VPackageEntity>( vPackageId.PackageManager, $"{vPackageId.Id}|{vPackageId.Version}" );
            TableResult tableResult = await VPackageTable.ExecuteAsync( retrieve );
            VPackageEntity entity = ( VPackageEntity )tableResult.Result;
            if( entity == null ) return null;

            if( string.IsNullOrWhiteSpace( entity.Dependencies ) ) return new VPackage( vPackageId );

            List<VPackage> dependencies = new List<VPackage>();
            foreach( VPackageId dependency in DeserializeVPackageDependencies( entity.Dependencies ) )
            {
                VPackage vPackage = await GetVPackage( dependency );
                if( vPackage != null ) dependencies.Add( vPackage );
            }

            VPackage result = new VPackage( vPackageId, dependencies );
            await CacheVPackage( result );
            return result;
        }

        async Task CacheVPackage( VPackage vPackage )
        {
            CloudBlockBlob blob = CloudBlobContainer.GetBlockBlobReference( $"{vPackage.VPackageId.PackageManager}/{vPackage.VPackageId.Id}/{vPackage.VPackageId.Version}" );
            string serializedVPackage = SerializeVPackage( vPackage ).ToString();
            await blob.UploadTextAsync( serializedVPackage );
        }

        async Task<Tuple<bool, VPackage>> GetCachedVPackage( VPackageId vPackageId )
        {
            CloudBlockBlob blob = CloudBlobContainer.GetBlockBlobReference( $"{vPackageId.PackageManager}/{vPackageId.Id}/{vPackageId.Version}" );
            if( !await blob.ExistsAsync() ) return Tuple.Create( false, null as VPackage );
            if( DateTime.UtcNow - blob.Properties.LastModified > _cacheTimeOut ) return Tuple.Create( false, null as VPackage );

            string serializedVPackage = await blob.DownloadTextAsync();

            VPackage result = DeserializeVPackage( serializedVPackage );
            return Tuple.Create( true, result );
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

        static IReadOnlyCollection<VPackageId> DeserializeVPackageDependencies( string dependencies )
        {
            return XElement.Parse( dependencies )
                .Descendants()
                .Select( d => new VPackageId(
                     d.Attribute( "PackageManager" ).Value,
                     d.Attribute( "Id" ).Value,
                     d.Attribute( "Version" ).Value ) )
                .ToList();
        }

        XElement SerializeVPackage( VPackage vPackage )
        {
            return new XElement(
                "VPackage",
                new XAttribute( "PackageManager", vPackage.VPackageId.PackageManager ),
                new XAttribute( "Id", vPackage.VPackageId.Id ),
                new XAttribute( "Version", vPackage.VPackageId.Version ),
                new XElement( "Dependencies", vPackage.Dependencies.Select( p => SerializeVPackage( p ) ) ) );
        }

        VPackage DeserializeVPackage( string vPackage )
        {
            XElement xElement = XElement.Parse( vPackage );
            return DeserializeVPackage( xElement );
        }

        VPackage DeserializeVPackage( XElement xElement )
        {
            return new VPackage(
                new VPackageId(
                    xElement.Attribute( "PackageManager" ).Value,
                    xElement.Attribute( "Id" ).Value,
                    xElement.Attribute( "Version" ).Value ),
                xElement.Element( "Dependencies" ).Elements( "VPackage" ).Select( x => DeserializeVPackage( x ) ).ToList() );
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
            get { return _packageTable ?? ( _packageTable = CloudTableClient.GetTableReference( _packageTableName ) ); }
        }

        CloudTable _vPackageTable;
        CloudTable VPackageTable
        {
            get { return _vPackageTable ?? ( _vPackageTable = CloudTableClient.GetTableReference( _vPackageTableName ) ); }
        }

        CloudTable _notCrawledVPackageTable;
        CloudTable NotCrawledVPackageTable
        {
            get { return _notCrawledVPackageTable ?? ( _notCrawledVPackageTable = CloudTableClient.GetTableReference( _notCrawledTableName ) ); }
        }

        CloudBlobClient _cloudBlobClient;
        CloudBlobClient CloudBlobClient
        {
            get { return _cloudBlobClient ?? ( _cloudBlobClient = CloudStorageAccount.CreateCloudBlobClient() ); }
        }

        CloudBlobContainer _cloudBlobContainer;
        CloudBlobContainer CloudBlobContainer
        {
            get { return _cloudBlobContainer ?? ( _cloudBlobContainer = CloudBlobClient.GetContainerReference( _vPackageCacheBlobContainerName ) ); }
        }



        public class PackageEntity : TableEntity
        {
            public PackageEntity()
            {
            }

            public PackageEntity( PackageId packageId )
            {
                PartitionKey = packageId.PackageManager;
                RowKey = packageId.Value;
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
                RowKey = $"{vPackageId.Id}|{vPackageId.Version}";
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
                RowKey = $"{vPackageId.Id}|{vPackageId.Version}";
            }
        }
    }
}
