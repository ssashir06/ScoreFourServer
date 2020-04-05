using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Adapter
{
    public interface IWaitingPlayerAdapter
    {
        Task<Player> DequeueAsync(CancellationToken cancellationToken);
        Task EnqueueAsync(Player player, DateTimeOffset timeOut, CancellationToken cancellationToken);
    }
}
