using BlogApp.BL.Dtos.StoryDtos;
using BlogApp.BL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlogApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController(IStoryService _service) : ControllerBase
    {
        /// <summary>
        /// Get all active stories
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveStories()
        {
            return Ok(await _service.GetActiveStoriesAsync());
        }

        /// <summary>
        /// Create a new story
        /// </summary>
        /// <remarks>
        /// Example request (form-data):
        /// 
        ///     POST /api/stories
        ///     {
        ///         "mediaFile": [binary file content]
        ///     }
        /// </remarks>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateStory([FromForm] StoryCreateDto dto)
        {
            await _service.CreateStoryAsync(dto);
            return Ok();
        }
    }
}
