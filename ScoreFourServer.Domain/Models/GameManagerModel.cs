using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.Exceptions;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Models
{
    public class GameManagerModel
    {
        private readonly IGameMovementAdapter gameMovementAdapter;

        public GameManagerModel(
            IGameMovementAdapter gameMovementAdapter,
            GameRoom gameRoom)
        {
            this.gameMovementAdapter = gameMovementAdapter;
            this.GameRoom = gameRoom;
            this.PlayerNumber = 1;
            this.Counter = 0;
        }

        public GameRoom GameRoom { get; }
        public int PlayerNumber { get; set; }
        public int Counter { get; set; }

        public async Task MoveAsync(Movement movement, CancellationToken cancellationToken)
        {
            if (movement == null)
            {
                throw new ArgumentNullException(nameof(movement));
            }
            if (this.GameRoom.GameRoomId != movement.GameRoomId)
            {
                throw new GameException("Wrong game room id");
            }
            if (this.GameRoom.Players[this.PlayerNumber - 1].GameUserId != movement.PlayerId)
            {
                throw new GameException("Wrong game player id");
            }
            if (this.Counter != movement.Counter)
            {
                throw new GameException("Wrong game counter number");
            }

            await gameMovementAdapter.AddAsync(GameRoom, movement, cancellationToken);
            this.Counter++;
            this.PlayerNumber = this.PlayerNumber == 1 ? 2 : 1;
        }

        public async Task<Movement> GetMovementAsync(int counter, CancellationToken cancellationToken)
        {
            return await gameMovementAdapter.GetAsync(this.GameRoom, counter, cancellationToken);
        }

        public async Task<bool> IsEndedAsync(CancellationToken cancellationToken)
        {
            // FIXME: Use game room status instead.
            return false;

            var now = DateTimeOffset.UtcNow;
            if (now - this.GameRoom.CreateDate > TimeSpan.FromMinutes(10))
            {
                return true;
            }
            var movements = await gameMovementAdapter.GetListAsync(GameRoom, cancellationToken);
            if (!movements.Any()
                && now - this.GameRoom.CreateDate > TimeSpan.FromMinutes(1))
            {
                return true;
            }
            if (movements.Any()
                && now - movements.Max(m => m.CreateDate) > TimeSpan.FromMinutes(1))
            {
                return true;
            }
            return false;
        }
    }
}
