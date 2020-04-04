using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.OnMemory.Adapter
{
    public class GameRoomAdapter : IGameRoomAdapter
    {
        internal static List<GameRoom> GameRooms { get; } = new List<GameRoom>();

        public async Task AddAsync(GameRoom gameRoom, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            GameRooms.Add(gameRoom);
        }

        public async Task<GameRoom> GetAsync(Guid gameRoomId, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            return GameRooms.FirstOrDefault(m => m.GameRoomId == gameRoomId);
        }

        public async Task<GameRoom> GetLatestByPlayerAsync(Player player, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            return GameRooms.OrderByDescending(m => m.CreateDate).FirstOrDefault(m => m.Players.Select(p => p.GameUserId).Contains(player.GameUserId));
        }

    }
}
