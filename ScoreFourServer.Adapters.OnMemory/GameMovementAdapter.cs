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
    public class GameMovementAdapter : IGameMovementAdapter
    {
        internal static List<Movement> Movements { get; } = new List<Movement>();
        public async Task<Movement> GetAsync(GameRoom gameRoom, int counter, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (Movements)
            {
                return Movements.FirstOrDefault(m =>
                    m.GameRoomId == gameRoom.GameRoomId && m.Counter == counter);
            }
        }

        public async Task SaveAsync(GameRoom gameRoom, Movement movement, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (Movements)
            {
                Movements.RemoveAll(m => m.GameRoomId == movement.GameRoomId && m.Counter == movement.Counter);
                Movements.Add(movement);
            }
        }

        public async Task<IList<Movement>> GetListAsync(GameRoom gameRoom, CancellationToken cancellationToken)
        {
            await Dummy.Delay(cancellationToken);
            lock (Movements)
            {
                return Movements.Where(m => m.GameRoomId == gameRoom.GameRoomId).ToList();
            }
        }
    }
}
