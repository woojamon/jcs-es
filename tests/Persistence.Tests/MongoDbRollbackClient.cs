using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using MongoDB.Driver.Core.Clusters;

namespace Persistence.Tests
{
    public class MongoDbRollbackClient : IMongoClient, IDisposable
    {
        public List<string> DatabaseNames = new List<string>();

        private MongoClient Client = new MongoClient();

        public ICluster Cluster => ((IMongoClient)Client).Cluster;

        public MongoClientSettings Settings => ((IMongoClient)Client).Settings;

        public void Dispose()
        {
            foreach (var databaseName in DatabaseNames)
                this.Client.DropDatabase(databaseName);
        }

        public void DropDatabase(string name, CancellationToken cancellationToken = default)
        {
            ((IMongoClient)Client).DropDatabase(name, cancellationToken);
        }

        public void DropDatabase(IClientSessionHandle session, string name, CancellationToken cancellationToken = default)
        {
            ((IMongoClient)Client).DropDatabase(session, name, cancellationToken);
        }

        public Task DropDatabaseAsync(string name, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).DropDatabaseAsync(name, cancellationToken);
        }

        public Task DropDatabaseAsync(IClientSessionHandle session, string name, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).DropDatabaseAsync(session, name, cancellationToken);
        }

        public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null)
        {
            DatabaseNames.Add(name);
            return ((IMongoClient)Client).GetDatabase(name, settings);
        }

        public IAsyncCursor<string> ListDatabaseNames(CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabaseNames(cancellationToken);
        }

        public IAsyncCursor<string> ListDatabaseNames(ListDatabaseNamesOptions options, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabaseNames(options, cancellationToken);
        }

        public IAsyncCursor<string> ListDatabaseNames(IClientSessionHandle session, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabaseNames(session, cancellationToken);
        }

        public IAsyncCursor<string> ListDatabaseNames(IClientSessionHandle session, ListDatabaseNamesOptions options, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabaseNames(session, options, cancellationToken);
        }

        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabaseNamesAsync(cancellationToken);
        }

        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(ListDatabaseNamesOptions options, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabaseNamesAsync(options, cancellationToken);
        }

        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabaseNamesAsync(session, cancellationToken);
        }

        public Task<IAsyncCursor<string>> ListDatabaseNamesAsync(IClientSessionHandle session, ListDatabaseNamesOptions options, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabaseNamesAsync(session, options, cancellationToken);
        }

        public IAsyncCursor<BsonDocument> ListDatabases(CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabases(cancellationToken);
        }

        public IAsyncCursor<BsonDocument> ListDatabases(ListDatabasesOptions options, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabases(options, cancellationToken);
        }

        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabases(session, cancellationToken);
        }

        public IAsyncCursor<BsonDocument> ListDatabases(IClientSessionHandle session, ListDatabasesOptions options, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabases(session, options, cancellationToken);
        }

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabasesAsync(cancellationToken);
        }

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(ListDatabasesOptions options, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabasesAsync(options, cancellationToken);
        }

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabasesAsync(session, cancellationToken);
        }

        public Task<IAsyncCursor<BsonDocument>> ListDatabasesAsync(IClientSessionHandle session, ListDatabasesOptions options, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).ListDatabasesAsync(session, options, cancellationToken);
        }

        public IClientSessionHandle StartSession(ClientSessionOptions options = null, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).StartSession(options, cancellationToken);
        }

        public Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = null, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).StartSessionAsync(options, cancellationToken);
        }

        public IChangeStreamCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).Watch(pipeline, options, cancellationToken);
        }

        public IChangeStreamCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).Watch(session, pipeline, options, cancellationToken);
        }

        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).WatchAsync(pipeline, options, cancellationToken);
        }

        public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
        {
            return ((IMongoClient)Client).WatchAsync(session, pipeline, options, cancellationToken);
        }

        public IMongoClient WithReadConcern(ReadConcern readConcern)
        {
            return ((IMongoClient)Client).WithReadConcern(readConcern);
        }

        public IMongoClient WithReadPreference(ReadPreference readPreference)
        {
            return ((IMongoClient)Client).WithReadPreference(readPreference);
        }

        public IMongoClient WithWriteConcern(WriteConcern writeConcern)
        {
            return ((IMongoClient)Client).WithWriteConcern(writeConcern);
        }
    }
}