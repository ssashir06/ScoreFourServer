using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.Factories;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Services
{
    public class PlayerMatchingService
    {
        private readonly IGameRoomAdapter gameRoomAdapter;
        private readonly IWaitingPlayerAdapter waitingPlayerAdapter;
        private readonly IClientTokenAdapter clientTokenAdapter;
        private readonly GameManagerFactory gameManagerFactory;

        public PlayerMatchingService(
            IGameRoomAdapter gameRoomAdapter,
            IWaitingPlayerAdapter waitingPlayerAdapter,
            IClientTokenAdapter clientTokenAdapter,
            GameManagerFactory gameManagerFactory
            )
        {
            this.gameRoomAdapter = gameRoomAdapter;
            this.waitingPlayerAdapter = waitingPlayerAdapter;
            this.clientTokenAdapter = clientTokenAdapter;
            this.gameManagerFactory = gameManagerFactory;
        }

        public async Task AddGamePlayer(Client player, CancellationToken cancellationToken)
        {
            var waitingPlayer = await this.waitingPlayerAdapter.DequeueAsync(cancellationToken);
            if (waitingPlayer == null || waitingPlayer.ClientId == player.ClientId)
            {
                await waitingPlayerAdapter.EnqueueAsync(player, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(10), cancellationToken);
            }
            else
            {
                var newGameRoom = new GameRoom
                {
                    CreateDate = DateTimeOffset.Now,
                    GameRoomId = Guid.NewGuid(),
                    Name = $"Game room {DateTimeOffset.UtcNow:F}",
                    Players = new[] { waitingPlayer, player },
                    GameRoomStatus = GameRoomStatus.Created,
                };
                await gameRoomAdapter.SaveAsync(newGameRoom, cancellationToken);
            }
        }

        public async Task<(GameRoom, ClientToken)> MatchAsync(Client player, CancellationToken cancellationToken)
        {
            var createdGameRoom = await gameRoomAdapter.GetLatestCreatedByPlayerAsync(player, cancellationToken);
            if (createdGameRoom != null)
            {
                var gameManager = await gameManagerFactory.FactoryAsync(createdGameRoom, cancellationToken);
                await gameManager.UpdateGameRoomStatusAsync(cancellationToken);
                if (gameManager.GameRoom.GameRoomStatus == GameRoomStatus.Created)
                {
                    var token = new ClientToken(
                        createdGameRoom.Players.First(m => m.ClientId == player.ClientId).GameUserId,
                        player.ClientId, Guid.NewGuid(), DateTimeOffset.UtcNow + TimeSpan.FromHours(10));
                    await clientTokenAdapter.SaveAsync(token, cancellationToken);

                    return (createdGameRoom, token);
                }
            }

            return (null, null);
        }
    }
}
