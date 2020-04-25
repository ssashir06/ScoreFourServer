using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Adapters
{
    public interface IWaitingPlayerAdapter
    {
        Task<Client> DequeueAsync(CancellationToken cancellationToken);
        Task EnqueueAsync(Client player, DateTimeOffset timeout, CancellationToken cancellationToken);
    }
}
