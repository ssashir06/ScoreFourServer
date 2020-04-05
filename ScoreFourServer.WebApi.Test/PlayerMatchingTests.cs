using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.Factories;
using ScoreFourServer.Domain.Services;
using ScoreFourServer.OnMemory.Adapter;
using ScoreFourServer.WebApi.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ScoreFourServer.WebApi.Test
{
    public class PlayerMatchingTests
    {
        private ServiceProvider serviceProvider;

        public PlayerMatchingTests()
        {
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddScoped(sp => new GameManagerFactory(
                sp.GetService<IGameMovementAdapter>()
                ));
            services.AddScoped(sp => new PlayerMatchingService(
                sp.GetService<IGameRoomAdapter>(),
                sp.GetService<IWaitingPlayerAdapter>(),
                sp.GetService<GameManagerFactory>()
                ));
            services.AddScoped<IGameMovementAdapter>(sp => new GameMovementAdapter());
            services.AddScoped<IGameRoomAdapter>(sp => new GameRoomAdapter());
            services.AddScoped<IWaitingPlayerAdapter>(sp => new WaitingPlayerAdapter());

            this.serviceProvider = services.BuildServiceProvider();
        }

        private void ResetAdapters()
        {
            GameRoomAdapter.GameRooms.Clear();
            GameMovementAdapter.Movements.Clear();
            WaitingPlayerAdapter.WaitingPlayers.Clear();
        }

        [Fact]
        public async void Test_Two_players_try_to_start_a_game()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PlayerMatchingController>>();
            var ctx = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            var controller = new PlayerMatchingController(
                mockLogger.Object,
                serviceProvider.GetService<PlayerMatchingService>()
                )
            {
                ControllerContext = ctx,
            };
            var playerGuids = new []
            {
                Guid.NewGuid(), Guid.NewGuid(),
            };
            ResetAdapters();

            // Act
            var results = new []
            {
                await controller.MatchAsync(playerGuids[0]),
                await controller.MatchAsync(playerGuids[1]),
            };

            // Assert
            // ... For player 1
            {
                var actionResult = Assert.IsType<AcceptedResult>(results[0]);
                Assert.Equal(202, actionResult.StatusCode);
            }
            // ... For player 2
            {
                var actionResult = Assert.IsType<JsonResult>(results[1]);
                var model = Assert.IsAssignableFrom<GameRoom>(actionResult.Value);
                Assert.Equal(playerGuids[0], model.Players[0].GameUserId);
                Assert.Equal(playerGuids[1], model.Players[1].GameUserId);
            }
        }
    }
}
