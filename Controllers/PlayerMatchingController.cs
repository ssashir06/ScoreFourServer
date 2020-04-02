using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScoreFourServer.Domain.Services;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreFourServer.Controllers
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

        [HttpGet("Match")]
        public async Task<ActionResult> MatchAsync(Guid gameUserId, string name)
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
                return new JsonResult(gameRoom);
            }
            else
            {
                return Accepted();
            }
        }
    }
}
