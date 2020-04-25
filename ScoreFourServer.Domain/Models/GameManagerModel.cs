using ScoreFourServer.Domain.Adapters;
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
        private readonly IGameRoomAdapter gameRoomAdapter;
        private readonly IGameMovementAdapter gameMovementAdapter;

        public GameManagerModel(
            IGameRoomAdapter gameRoomAdapter,
            IGameMovementAdapter gameMovementAdapter,
            GameRoom gameRoom)
        {
            this.gameRoomAdapter = gameRoomAdapter;
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
            if (this.GameRoom.Players[this.PlayerNumber - 1].GameUserId != movement.GameUserId
                || this.GameRoom.Players[movement.Counter % 2].GameUserId != movement.GameUserId)
            {
                throw new GameException("Wrong game player id");
            }
            if (this.Counter != movement.Counter)
            {
                throw new GameException("Wrong game counter number");
            }

            await gameMovementAdapter.SaveAsync(movement, cancellationToken);

            if (GameRoom.GameRoomStatus == GameRoomStatus.Created)
            {
                GameRoom.GameRoomStatus = GameRoomStatus.Started;
                await gameRoomAdapter.SaveAsync(GameRoom, cancellationToken);
            }
            else
            {
                await CheckTimeout(cancellationToken);
            }

            this.Counter++;
            this.PlayerNumber = this.PlayerNumber == 1 ? 2 : 1;
        }

        public async Task<Movement> GetMovementAsync(int counter, CancellationToken cancellationToken)
        {
            return await gameMovementAdapter.GetAsync(this.GameRoom, counter, cancellationToken);
        }

        public async Task UpdateGameRoomStatusAsync(CancellationToken cancellationToken)
        {
            await CheckTimeout(cancellationToken);
        }

        private async Task CheckTimeout(CancellationToken cancellationToken)
        {
            if (GameRoom.GameRoomStatus == GameRoomStatus.Timedout)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var movements = await gameMovementAdapter.GetListAsync(GameRoom, cancellationToken);
            if (!movements.Any()
                && now - this.GameRoom.CreateDate > TimeSpan.FromMinutes(5))
            {
                GameRoom.GameRoomStatus = GameRoomStatus.Timedout;
            }
            if (movements.Any()
                && now - movements.Max(m => m.CreateDate) > TimeSpan.FromMinutes(5))
            {
                GameRoom.GameRoomStatus = GameRoomStatus.Timedout;
            }

            if (GameRoom.GameRoomStatus == GameRoomStatus.Timedout)
            {
                await gameRoomAdapter.SaveAsync(GameRoom, cancellationToken);
            }
        }

        public async Task UpdateWinnerAsync(Guid gameUserId, CancellationToken ct)
        {
            if (!GameRoom.Players.Select(m => m.GameUserId).Contains(gameUserId))
            {
                throw new ArgumentException(nameof(gameUserId));
            }
            if (GameRoom.GameRoomStatus != GameRoomStatus.Started)
            {
                throw new GameException("Game is not started.");
            }

            GameRoom.GameRoomStatus = GameRoomStatus.GameOver;
            GameRoom.WinnerGameUserId = gameUserId;
            await gameRoomAdapter.SaveAsync(GameRoom, ct);
        }

        public async Task LeaveGameAsync(Guid gameUserId, CancellationToken ct)
        {
            if (new[] { GameRoomStatus.GameOver, GameRoomStatus.Timedout }.Contains(GameRoom.GameRoomStatus))
            {
                return;
            }

            GameRoom.GameRoomStatus = GameRoomStatus.Left;
            await gameRoomAdapter.SaveAsync(GameRoom, ct);
        }
    }
}
