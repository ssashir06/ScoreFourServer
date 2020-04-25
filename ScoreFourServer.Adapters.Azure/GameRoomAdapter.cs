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
    public class GameRoomAdapter : IGameRoomAdapter
    {
        const string tableName = "gameroom";

        public GameRoomAdapter(string connectionString)
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

        public async Task<GameRoom> GetAsync(Guid gameRoomId, CancellationToken cancellationToken)
        {
            try
            {
                var table = await GetTableAsync(cancellationToken);
                var query = new TableQuery<GameRoomTableEntity>()
                    .Where(TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition(nameof(GameRoomTableEntity.RowKey), QueryComparisons.Equal, gameRoomId.ToString("D")),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForGuid(nameof(GameRoomTableEntity.GameRoomId), QueryComparisons.Equal, gameRoomId)
                        ));
                var entity = await Tools.CosmosTableTool.GetEntitiesAsync(table, query, cancellationToken).FirstOrDefaultAsync(cancellationToken);
                return entity != null ? (GameRoom)entity : null;
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task<GameRoom> GetLatestCreatedByPlayerAsync(Client player, CancellationToken cancellationToken)
        {
            try
            {
                var table = await GetTableAsync(cancellationToken);
                var query = new TableQuery<GameRoomTableEntity>()
                    .Where(TableQuery.CombineFilters(
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterConditionForGuid(nameof(GameRoomTableEntity.Player1ClientId), QueryComparisons.Equal, player.ClientId),
                            TableOperators.Or,
                            TableQuery.GenerateFilterConditionForGuid(nameof(GameRoomTableEntity.Player2ClientId), QueryComparisons.Equal, player.ClientId)
                            ),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition(nameof(GameRoomTableEntity.GameRoomStatus), QueryComparisons.Equal, GameRoomStatus.Created.ToString())
                        ));
                var entity = await Tools.CosmosTableTool.GetEntitiesAsync(table, query, cancellationToken)
                    .OrderByDescending(m => m.Timestamp)
                    .FirstOrDefaultAsync(cancellationToken);
                return entity != null ? (GameRoom)entity : null;
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task SaveAsync(GameRoom gameRoom, CancellationToken cancellationToken)
        {
            try
            {
                var entity = (GameRoomTableEntity)gameRoom;
                var operation = TableOperation.InsertOrMerge(entity);
                var table = await GetTableAsync(cancellationToken);
                var result = await table.ExecuteAsync(operation, cancellationToken);
                var inserted = result.Result as GameRoomTableEntity;
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }
    }
}
