using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScoreFourServer.Domain.Services;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet("")]
        public async Task<IActionResult> MatchAsync(Guid gameUserId, string name)
        {
            var player = new Player
            {
                GameUserId = gameUserId,
                Name = name,
            };
            var ct = HttpContext.RequestAborted;
            var gameRoom = await playerMatchingService.MatchAsync(player, ct);
            if (gameRoom != null)
            {
                logger.LogInformation($"Matching found: Player1={gameRoom.Players[0].Name}, Player2={gameRoom.Players[1].Name}");
                return new JsonResult(gameRoom);
            }
            else
            {
                return Accepted();
            }
        }
    }
}
