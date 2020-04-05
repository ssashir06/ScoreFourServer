﻿using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.Factories;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Services
{
    public class PlayerMatchingService
    {
        private readonly IGameRoomAdapter gameRoomAdapter;
        private readonly IWaitingPlayerAdapter waitingPlayerAdapter;
        private readonly GameManagerFactory gameManagerFactory;

        public PlayerMatchingService(
            IGameRoomAdapter gameRoomAdapter,
            IWaitingPlayerAdapter waitingPlayerAdapter,
            GameManagerFactory gameManagerFactory
            )
        {
            this.gameRoomAdapter = gameRoomAdapter;
            this.waitingPlayerAdapter = waitingPlayerAdapter;
            this.gameManagerFactory = gameManagerFactory;
        }

        public async Task AddGamePlayer(Player player, CancellationToken cancellationToken)
        {
            var waitingPlayer = await this.waitingPlayerAdapter.DequeueAsync(cancellationToken);
            if (waitingPlayer == null)
            {
                await waitingPlayerAdapter.EnqueueAsync(player, cancellationToken);
            }
            else
            {
                var newGameRoom = new GameRoom
                {
                    CreateDate = DateTimeOffset.Now,
                    GameRoomId = Guid.NewGuid(),
                    Name = $"Game room {DateTimeOffset.UtcNow:F}",
                    Players = new[] { waitingPlayer, player },
                };
                await gameRoomAdapter.AddAsync(newGameRoom, cancellationToken);
            }
        }

        public async Task<GameRoom> MatchAsync(Player player, CancellationToken cancellationToken)
        {
            var createdGameRoom = await gameRoomAdapter.GetLatestByPlayerAsync(player, cancellationToken);
            if (createdGameRoom != null)
            {
                var gameManager = await gameManagerFactory.FactoryAsync(createdGameRoom, cancellationToken);
                if (!await gameManager.IsEndedAsync(cancellationToken))
                {
                    return createdGameRoom;
                }
            }

            return null;
        }
    }
}