using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using ScoreFourServer.OnMemory.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.OnMemory.Adapter
{
    public class GameMovementAdapter : IGameMovementAdapter
    {
        internal static List<Movement> Movements { get; } = new List<Movement>();
        public async Task<Movement> GetAsync(GameRoom gameRoom, int counter, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            return Movements.FirstOrDefault(m => 
                m.GameRoomId == gameRoom.GameRoomId && m.Counter == counter);
        }

        public async Task AddAsync(GameRoom gameRoom, Movement movement, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            Movements.Add(movement);
        }

        public async Task<IList<Movement>> GetListAsync(GameRoom gameRoom, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            return Movements.Where(m => m.GameRoomId == gameRoom.GameRoomId).ToList();
        }
    }
}
