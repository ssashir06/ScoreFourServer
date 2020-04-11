using ScoreFourServer.Domain.Adapters;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Domain.Factories
{
    public class GameManagerFactory
    {
        private readonly IGameRoomAdapter gameRoomAdapter;
        private readonly IGameMovementAdapter gameMovementAdapter;

        public GameManagerFactory(
            IGameRoomAdapter gameRoomAdapter,
            IGameMovementAdapter gameMovementAdapter
            )
        {
            this.gameRoomAdapter = gameRoomAdapter;
            this.gameMovementAdapter = gameMovementAdapter;
        }
        
        public async Task<GameManagerModel> FactoryAsync(GameRoom gameRoom, CancellationToken cancellationToken)
        {
            var movements = await gameMovementAdapter.GetListAsync(gameRoom, cancellationToken);

            return new GameManagerModel(
                gameRoomAdapter,
                gameMovementAdapter,
                gameRoom
                )
            {
                Counter = movements.Count,
                PlayerNumber = movements.Count % 2 + 1,
            };
        }
    }
}
