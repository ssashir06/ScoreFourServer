using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScoreFourServer.Domain.Adapter;
using ScoreFourServer.Domain.Exceptions;
using ScoreFourServer.Domain.Factories;
using ScoreFourServer.Domain.ValueObject;
using ScoreFourServer.WebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreFourServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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

        [HttpPut("{gameRoomId}/Movement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetMovementAsync(Guid gameRoomId, [FromBody]MovementPutVM movementPatch)
        {
            var ct = HttpContext.RequestAborted;
            var gameRoom = await gameRoomAdapter.GetAsync(gameRoomId, ct);
            if (gameRoom == null)
            {
                return BadRequest("Invalid game room number.");
            }
            var gameManager = await gameManagerFactory.FactoryAsync(gameRoom, ct);
            try
            {
                var playerId = gameRoom.Players[movementPatch.PlayerNumber - 1].GameUserId;
                var movement = new Movement(
                    movementPatch.X, movementPatch.Y, movementPatch.Counter,
                    playerId, gameRoomId, DateTimeOffset.Now
                    );
                await gameManager.MoveAsync(movement, ct);
            }
            catch (GameException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet("{gameRoomId}/Movement/{counter}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMovementAsync(Guid gameRoomId, int counter)
        {

            var ct = HttpContext.RequestAborted;
            var gameRoom = await gameRoomAdapter.GetAsync(gameRoomId, ct);
            if (gameRoom == null)
            {
                return BadRequest("Invalid game room number.");
            }
            var gameManager = await gameManagerFactory.FactoryAsync(gameRoom, ct);
            Movement movement;
            try
            {
                movement = await gameManager.GetMovementAsync(counter, ct);
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

        [HttpGet("{gameRoomId}/IsEnded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IsEndedAsync(Guid gameRoomId)
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

        [HttpGet("{gameRoomId}/Status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGameStatus(Guid gameRoomId)
        {
            var ct = HttpContext.RequestAborted;
            var gameRoom = await gameRoomAdapter.GetAsync(gameRoomId, ct);
            if (gameRoom == null)
            {
                return BadRequest("Invalid game room number.");
            }
            return new JsonResult(gameRoom);

        }
    }
}
