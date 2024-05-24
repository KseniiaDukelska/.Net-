using Microsoft.AspNetCore.Mvc;
using Rocky.Services;
using Rocky_Models.Models;
using System.Security.Claims;

namespace Rocky.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InteractionsController : ControllerBase
    {
        private readonly IUserInteractionService _userInteractionService;

        public InteractionsController(IUserInteractionService userInteractionService)
        {
            _userInteractionService = userInteractionService;
        }

        [HttpPost("log")]
        public IActionResult LogInteraction([FromBody] UserInteraction interaction)
        {
            if (interaction == null)
            {
                return BadRequest("Interaction data is null.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not logged in.");
            }

            interaction.UserId = int.Parse(userId); // Set the UserId from the authenticated user
            interaction.InteractionTime = DateTime.Now; // Set the current time

            _userInteractionService.LogInteraction(interaction);
            return Ok("Interaction logged successfully.");
        }
    }
}
