using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScoreFourServer.Domain.Services;
using ScoreFourServer.Domain.ValueObject;
using ScoreFourServer.WebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PlayerMatchingController : ControllerBase
    {
        private readonly ILogger<PlayerMatchingController> logger;
        private readonly PlayerMatchingService playerMatchingService;

        public PlayerMatchingController(
            ILogger<PlayerMatchingController> logger,
            PlayerMatchingService playerMatchingService
            )
        {
            this.logger = logger;
            this.playerMatchingService = playerMatchingService;
        }

        [HttpPut("GamePlayer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddGamePlayer(GamePlayerPutVM gamePlayer)
        {
            var player = new Client(gamePlayer.GameUserId, gamePlayer.ClientId, gamePlayer.Name);
            var ct = HttpContext.RequestAborted;
            await playerMatchingService.AddGamePlayer(player, ct);

            return Ok();
        }

        [HttpGet("GamePlayer/{clientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> MatchAsync(Guid clientId)
        {
            var player = new Client(Guid.Empty, clientId, null);
            var ct = HttpContext.RequestAborted;
            var (gameRoom, clientToken) = await playerMatchingService.MatchAsync(player, ct);
            if (gameRoom != null)
            {
                logger.LogInformation($"Matching found: Player1={gameRoom.Players[0].Name} {gameRoom.Players[0].ClientId}, Player2={gameRoom.Players[1].Name} {gameRoom.Players[1].ClientId}");
                var token = Convert.ToBase64String(clientToken.Token.ToByteArray());
                return new JsonResult(new
                {
                    GameRoom = new GameRoomGetVM
                    {
                        GameRoomId = gameRoom.GameRoomId,
                        Name = gameRoom.Name,
                        Players = gameRoom.Players.Select(m => m.Name).ToArray(),
                    },
                    Token = token
                });
            }
            else
            {
                return Accepted();
            }
        }
    }
}
