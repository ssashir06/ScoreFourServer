using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Adapters
{
    public interface IGameRoomAdapter
    {
        Task<GameRoom> GetAsync(Guid gameRoomId, CancellationToken cancellationToken);
        Task<GameRoom> GetLatestCreatedByPlayerAsync(Client player, CancellationToken cancellationToken);
        Task SaveAsync(GameRoom gameRoom, CancellationToken cancellationToken);
    }
}
