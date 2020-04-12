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
            var player = new Player(gamePlayer.GameUserId, gamePlayer.Name);
            var ct = HttpContext.RequestAborted;
            await playerMatchingService.AddGamePlayer(player, ct);

            return Ok();
        }

        [HttpGet("GamePlayer/{gameUserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> MatchAsync(Guid gameUserId)
        {
            var player = new Player(gameUserId, null);
            var ct = HttpContext.RequestAborted;
            var gameRoom = await playerMatchingService.MatchAsync(player, ct);
            if (gameRoom != null)
            {
                logger.LogInformation($"Matching found: Player1={gameRoom.Players[0].Name} {gameRoom.Players[0].GameUserId}, Player2={gameRoom.Players[1].Name} {gameRoom.Players[1].GameUserId}");
                return new JsonResult(gameRoom);
            }
            else
            {
                return Accepted();
            }
        }
    }
}
