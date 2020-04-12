using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using ScoreFourServer.Adapters.OnMemory.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Adapters.OnMemory
{
    public class GameRoomAdapter : IGameRoomAdapter
    {
        internal static List<GameRoom> GameRooms { get; } = new List<GameRoom>();

        public async Task<GameRoom> GetAsync(Guid gameRoomId, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (GameRooms)
            {
                return GameRooms.FirstOrDefault(m => m.GameRoomId == gameRoomId);
            }
        }

        public async Task<GameRoom> GetLatestCreatedByPlayerAsync(Player player, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (GameRooms)
            {
                return GameRooms
                    .OrderByDescending(m => m.CreateDate)
                    .FirstOrDefault(m => m.Players.Select(p => p.GameUserId).Contains(player.GameUserId));
            }
        }

        public async Task SaveAsync(GameRoom gameRoom, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (GameRooms)
            {
                GameRooms.RemoveAll(m => m.GameRoomId == gameRoom.GameRoomId);
                GameRooms.Add(gameRoom);
            }
        }
    }
}
