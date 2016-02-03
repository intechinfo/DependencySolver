using System;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Invenietis.DependencyCrawler.IO
{
    public class AzureJobQueue : IJobQueue
    {
        readonly string _connectionString;
        readonly string _queueName;

        public AzureJobQueue( string connectionString, string queueName )
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task<IJob> PeekNextJob()
        {
            CloudQueueMessage message = await PeekMessage();
            IJob job = MessageToJob( message );
            return job;
        }

        public Task PutJob( IJob job )
        {
            string serializedJob = Serializer.Serialize( job );
            CloudQueueMessage message = new CloudQueueMessage( serializedJob );
            return CloudQueue.AddMessageAsync( message );
        }

        public async Task<IJob> TakeNextJob()
        {
            CloudQueueMessage message = await GetMessage();
            IJob job = MessageToJob( message );
            await CloudQueue.DeleteMessageAsync( message );
            return job;
        }

        Task<CloudQueueMessage> PeekMessage()
        {
            return PollQueue( q => q.PeekMessageAsync() );
        }

        Task<CloudQueueMessage> GetMessage()
        {
            return PollQueue( q => q.GetMessageAsync() );
        }

        async Task<CloudQueueMessage> PollQueue( Func<CloudQueue, Task<CloudQueueMessage>> accessMessage )
        {
            for( ;;)
            {
                CloudQueueMessage message = await accessMessage( CloudQueue );
                if( message != null ) return message;
                await Task.Delay( TimeSpan.FromSeconds( 10 ) );
            }
        }

        CloudStorageAccount _cloudStorageAccount;
        CloudStorageAccount CloudStorageAccout
        {
            get { return _cloudStorageAccount ?? ( _cloudStorageAccount = CloudStorageAccount.Parse( _connectionString ) ); }
        }

        CloudQueueClient _cloudQueueClient;
        CloudQueueClient CloudQueueClient
        {
            get { return _cloudQueueClient ?? ( _cloudQueueClient = CloudStorageAccout.CreateCloudQueueClient() ); }
        }

        CloudQueue _cloudQueue;
        CloudQueue CloudQueue
        {
            get { return _cloudQueue ?? ( _cloudQueue = CloudQueueClient.GetQueueReference( _queueName ) ); }
        }

        JobSerializer _jobSerializer;
        JobSerializer Serializer
        {
            get { return _jobSerializer ?? ( _jobSerializer = new JobSerializer() ); }
        }

        IJob MessageToJob( CloudQueueMessage message )
        {
            string[] messageParts = message.AsString.Split( '|' );
            string jobType = messageParts[ 0 ];
            if( jobType == "VPackageCrawled" ) return new VPackageCrawledJob( new VPackageId( messageParts[ 1 ], messageParts[ 2 ] ) );
            if( jobType == "Stop" ) return new StopJob();
            if( jobType == "PackageCrawled" ) return new PackageCrawledJob( new PackageId( messageParts[ 1 ] ) );
            if( jobType == "CrawlVPackage" ) return new CrawlVPackageJob( new VPackageId( messageParts[ 1 ], messageParts[ 2 ] ) );
            if( jobType == "CrawlPackage" ) return new CrawlPackageJob( new PackageId( messageParts[ 1 ] ) );

            throw new Exception( IOResources.UnknownMessageType );
        }

        public async Task Clear()
        {
            await CloudQueue.ClearAsync();
        }

        class JobSerializer : IJobVisitor
        {
            string _serializedJob;

            internal string Serialize( IJob job )
            {
                job.Accept( this );
                return _serializedJob;
            }

#pragma warning disable 1998

            public async Task Visit( VPackageCrawledJob job )
            {
                _serializedJob = $"VPackageCrawled|{job.VPackageId.Id}|{job.VPackageId.Version}";
            }

            public async Task Visit( StopJob stopJob )
            {
                _serializedJob = $"Stop";
            }

            public async Task Visit( PackageCrawledJob job )
            {
                _serializedJob = $"PackageCrawled|{job.PackageId.Id}";
            }

            public async Task Visit( CrawlVPackageJob job )
            {
                _serializedJob = $"CrawlVPackage|{job.VPackageId.Id}|{job.VPackageId.Version}";
            }

            public async Task Visit( CrawlPackageJob job )
            {
                _serializedJob = $"CrawlPackage|{job.PackageId.Id}";
            }

#pragma warning restore 1998

        }
    }
}
