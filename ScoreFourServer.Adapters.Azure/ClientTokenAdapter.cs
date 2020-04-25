using Microsoft.Azure.Cosmos.Table;
using ScoreFourServer.Adapters.Azure.TableEntities;
using ScoreFourServer.Domain;
using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Adapters.Azure
{
    public class ClientTokenAdapter : IClientTokenAdapter
    {
        const string tableName = "clienttoken";

        public ClientTokenAdapter(string connectionString)
        {
            StorageAccount = Tools.CosmosTableTool.CreateStorageAccount(connectionString);
        }

        public CloudStorageAccount StorageAccount { get; }

        private async Task<CloudTable> GetTableAsync(CancellationToken cancellationToken)
        {
            var tableClient = StorageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync(cancellationToken);
            return table;
        }

        public async Task<ClientToken> GetAsync(Guid token, CancellationToken cancellationToken)
        {
            try
            {
                var table = await GetTableAsync(cancellationToken);
                var query = new TableQuery<ClientTokenTableEntity>()
                    .Where(
                        TableQuery.GenerateFilterCondition(nameof(ClientTokenTableEntity.PartitionKey), QueryComparisons.Equal, token.ToString("D"))
                    );
                var entity = await Tools.CosmosTableTool.GetEntitiesAsync(table, query, cancellationToken).FirstOrDefaultAsync(cancellationToken);
                return entity != null ? (ClientToken)entity : null;
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task SaveAsync(ClientToken clientToken, CancellationToken cancellationToken)
        {
            try
            {
                var entity = (ClientTokenTableEntity)clientToken;
                var operation = TableOperation.InsertOrMerge(entity);
                var table = await GetTableAsync(cancellationToken);
                var result = await table.ExecuteAsync(operation, cancellationToken);
                var inserted = result.Result as ClientTokenTableEntity;
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }
    }
}
