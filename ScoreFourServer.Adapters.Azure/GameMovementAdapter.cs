using Microsoft.Azure.Cosmos.Table;
using ScoreFourServer.Adapters.Azure.TableEntities;
using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Adapters.Azure
{
    public class GameMovementAdapter : IGameMovementAdapter
    {
        const string tableName = "movement";

        public GameMovementAdapter(string connectionString)
        {
            StorageAccount = Tools.StorageTableTool.CreateStorageAccountFromConnectionString(connectionString);
        }

        public CloudStorageAccount StorageAccount { get; }

        private async Task<CloudTable> GetTable(CancellationToken cancellationToken)
        {
            var tableClient = StorageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync(cancellationToken);
            return table;
        }

        public async Task<Movement> GetAsync(GameRoom gameRoom, int counter, CancellationToken cancellationToken)
        {
            try
            {
                var table = await GetTable(cancellationToken);
                var query = new TableQuery<MovementTableEntity>()
                    .Where(TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition(nameof(MovementTableEntity.PartitionKey), QueryComparisons.Equal, gameRoom.GameRoomId.ToString("D")),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition(nameof(MovementTableEntity.RowKey), QueryComparisons.Equal, counter.ToString())
                        ));
                var entity = await Tools.StorageTableTool.GetEntitiesAsync(table, query, cancellationToken).FirstOrDefaultAsync(cancellationToken);
                return entity != null ? (Movement)entity : null;
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task<IList<Movement>> GetListAsync(GameRoom gameRoom, CancellationToken cancellationToken)
        {
            try
            {
                var table = await GetTable(cancellationToken);
                var query = new TableQuery<MovementTableEntity>()
                    .Where(
                        TableQuery.GenerateFilterCondition(nameof(MovementTableEntity.PartitionKey), QueryComparisons.Equal, gameRoom.GameRoomId.ToString("D"))
                        );
                var entites = await Tools.StorageTableTool.GetEntitiesAsync(table, query, cancellationToken).ToListAsync(cancellationToken);
                return entites.Select(m => (Movement)m).ToList();
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task SaveAsync(Movement movement, CancellationToken cancellationToken)
        {
            try
            {
                var entity = (MovementTableEntity)movement;
                var operation = TableOperation.InsertOrMerge(entity);
                var table = await GetTable(cancellationToken);
                var result = await table.ExecuteAsync(operation, cancellationToken);
                var inserted = result.Result as MovementTableEntity;
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }
    }
}
