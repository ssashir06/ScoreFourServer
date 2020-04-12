using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace ScoreFourServer.Adapters.Azure.Tools
{
    public class CosmosTableTool
    {
        public static CloudStorageAccount CreateStorageAccount(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Trace.TraceError("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Trace.TraceError("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }

            return storageAccount;
        }

        public static async IAsyncEnumerable<TEntity> GetEntitiesAsync<TEntity>(CloudTable table, TableQuery<TEntity> tableQuery, [EnumeratorCancellation]CancellationToken cancellationToken) where TEntity : class, ITableEntity, new()
        {
            TableContinuationToken token = null;
            do
            {
                var seg = await table.ExecuteQuerySegmentedAsync(tableQuery, token);
                token = seg.ContinuationToken;
                foreach (var item in seg)
                {
                    yield return item;
                }

            } while (token != null && !cancellationToken.IsCancellationRequested);
        }
    }
}
