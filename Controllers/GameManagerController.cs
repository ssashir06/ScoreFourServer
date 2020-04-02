using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.Exceptions;
using ScoreFourServer.Domain.Factories;
using ScoreFourServer.Domain.ValueObject;
using ScoreFourServer.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameManagerController : ControllerBase
    {
        private readonly ILogger<GameManagerController> logger;
        private readonly IGameRoomAdapter gameRoomAdapter;
        private readonly GameManagerFactory gameManagerFactory;

        public GameManagerController(
            ILogger<GameManagerController> logger,
            IGameRoomAdapter gameRoomAdapter,
            GameManagerFactory gameManagerFactory
            )
        {
            this.logger = logger;
            this.gameRoomAdapter = gameRoomAdapter;
            this.gameManagerFactory = gameManagerFactory;
        }

        [HttpPatch("Movement")]
        public async Task<ActionResult> SetMovementAsync([FromBody]MovementPatchVM movementPatch)
        {
            var ct = HttpContext.RequestAborted;
            var gameRoom = await gameRoomAdapter.GetAsync(movementPatch.GameRoomId, ct);
            if (gameRoom == null)
            {
                return BadRequest("Invalid game room number.");
            }
            var gameManager = await gameManagerFactory.FactoryAsync(gameRoom, ct);
            try
            {
                var movement = new Movement(
                    movementPatch.X, movementPatch.Y, movementPatch.Counter,
                    movementPatch.PlayerId, movementPatch.GameRoomId, DateTimeOffset.Now
                    );
                await gameManager.MoveAsync(movement, ct);
            }
            catch (GameException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet("Movement")]
        public async Task<ActionResult> GetMovementAsync(Guid gameRoomId, [Range(1, 2)]int playerNumber, int counter)
        {

            var ct = HttpContext.RequestAborted;
            var gameRoom = await gameRoomAdapter.GetAsync(gameRoomId, ct);
            if (gameRoom == null)
            {
                return BadRequest("Invalid game room number.");
            }
            var gameManager = await gameManagerFactory.FactoryAsync(gameRoom, ct);
            var player = gameRoom.Players[playerNumber - 1];
            Movement movement;
            try
            {
                movement = await gameManager.GetMovementAsync(player, counter, ct);
            }
            catch (GameException ex)
            {
                return BadRequest(ex.Message);
            }
            if (movement == null)
            {
                return NotFound();
            }
            return new JsonResult(movement);
        }

        [HttpGet("IsEnded")]
        public async Task<ActionResult> IsEndedAsync(Guid gameRoomId)
        {
            var ct = HttpContext.RequestAborted;
            var gameRoom = await gameRoomAdapter.GetAsync(gameRoomId, ct);
            if (gameRoom == null)
            {
                return BadRequest("Invalid game room number.");
            }
            var gameManager = await gameManagerFactory.FactoryAsync(gameRoom, ct);
            return new JsonResult(await gameManager.IsEndedAsync(ct));

        }
    }
}
