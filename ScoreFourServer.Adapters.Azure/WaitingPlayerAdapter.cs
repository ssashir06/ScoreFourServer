using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage;
using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;

namespace ScoreFourServer.Adapters.Azure
{
    public class WaitingPlayerAdapter : IWaitingPlayerAdapter
    {
        const string queueName = "waitingplayer";

        public WaitingPlayerAdapter(string connectionString)
        {
            StorageAccount = Tools.StorageTool.CreateStorageAccount(connectionString);
        }

        public CloudStorageAccount StorageAccount { get; }

        private async Task<CloudQueue> GetQueueAsync(CancellationToken cancellationToken)
        {
            var queueClient = StorageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            return queue;
        }

        public async Task<Client> DequeueAsync(CancellationToken cancellationToken)
        {
            try
            {
                var queue = await GetQueueAsync(cancellationToken);
                var message = await queue.GetMessageAsync();
                if (message == null)
                {
                    return null;
                }

                await queue.DeleteMessageAsync(message);
                var converter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(message.AsString, converter);
                return new Client(Guid.Parse(obj.GameUserId), Guid.Parse(obj.ClientId), obj.Name);
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        public async Task EnqueueAsync(Client player, DateTimeOffset timeout, CancellationToken cancellationToken)
        {
            try
            {
                var queue = await GetQueueAsync(cancellationToken);
                var str = JsonConvert.SerializeObject(player);
                var message = new CloudQueueMessage(str);
                await queue.AddMessageAsync(message,
                    timeToLive: timeout - DateTimeOffset.Now,
                    initialVisibilityDelay: TimeSpan.Zero,
                    null, null, cancellationToken);
            }
            catch (StorageException ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }
    }
}
