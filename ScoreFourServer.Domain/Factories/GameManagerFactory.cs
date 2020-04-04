using ScoreFourServer.Domain.Adapter;
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
        private readonly IGameMovementAdapter gameMovementAdapter;

        public GameManagerFactory(
            IGameMovementAdapter gameMovementAdapter
            )
        {
            this.gameMovementAdapter = gameMovementAdapter;
        }
        
        public async Task<GameManagerModel> FactoryAsync(GameRoom gameRoom, CancellationToken cancellationToken)
        {
            var movements = await gameMovementAdapter.GetListAsync(gameRoom, cancellationToken);

            return new GameManagerModel(
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
