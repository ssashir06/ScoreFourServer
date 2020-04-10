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
            if (this.GameRoom.Players[this.PlayerNumber - 1].GameUserId != movement.PlayerId)
            {
                throw new GameException("Wrong game player id");
            }
            if (this.Counter != movement.Counter)
            {
                throw new GameException("Wrong game counter number");
            }

            await gameMovementAdapter.AddAsync(GameRoom, movement, cancellationToken);

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
            if (GameRoom.GameRoomStatus == GameRoomStatus.TimedOut)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            var movements = await gameMovementAdapter.GetListAsync(GameRoom, cancellationToken);
            if (!movements.Any()
                && now - this.GameRoom.CreateDate > TimeSpan.FromMinutes(5))
            {
                GameRoom.GameRoomStatus = GameRoomStatus.TimedOut;
            }
            if (movements.Any()
                && now - movements.Max(m => m.CreateDate) > TimeSpan.FromMinutes(5))
            {
                GameRoom.GameRoomStatus = GameRoomStatus.TimedOut;
            }

            if (GameRoom.GameRoomStatus == GameRoomStatus.TimedOut)
            {
                await gameRoomAdapter.SaveAsync(GameRoom, cancellationToken);
            }
        }

        public async Task UpdateWinnerAsync(int playerNumber, CancellationToken ct)
        {
            if (GameRoom.GameRoomStatus == GameRoomStatus.GameOver && GameRoom.Winner == playerNumber)
            {
                return;
            }
            if (GameRoom.GameRoomStatus != GameRoomStatus.Started)
            {
                throw new ArgumentException(nameof(playerNumber));
            }

            GameRoom.GameRoomStatus = GameRoomStatus.GameOver;
            GameRoom.Winner = PlayerNumber;
            await gameRoomAdapter.SaveAsync(GameRoom, ct);
        }

        public async Task LeaveGameAsync(int playerNumber, CancellationToken ct)
        {
            if (new[] { GameRoomStatus.GameOver, GameRoomStatus.TimedOut }.Contains(GameRoom.GameRoomStatus))
            {
                return;
            }

            GameRoom.GameRoomStatus = GameRoomStatus.Left;
            await gameRoomAdapter.SaveAsync(GameRoom, ct);
        }
    }
}
