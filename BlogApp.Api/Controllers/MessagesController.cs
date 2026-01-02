using BlogApp.BL.Dtos.MessageDtos;
using BlogApp.BL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlogApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController(IMessageService _service) : ControllerBase
    {
        /// <summary>
        /// Get conversation with a specific user
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve the conversation with.</param>
        /// <returns>A list of messages in the conversation.</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConversation(string userId)
        {
            return Ok(await _service.GetConversationAsync(userId));
        }

        /// <summary>
        /// Send a message to another user
        /// </summary>
        /// <param name="dto">The message details.</param>
        /// <remarks>
        /// Example request:
        /// 
        ///     POST /api/messages
        ///     {
        ///         "receiverId": "user-id-123",
        ///         "content": "Hello! How are you doing?"
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessage(MessageCreateDto dto)
        {
            await _service.SendMessageAsync(dto);
            return Ok();
        }
    }
}
