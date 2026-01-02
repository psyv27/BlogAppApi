using BlogApp.BL.Dtos.BlogDtos;
using BlogApp.BL.Dtos.CommentDtos;
using BlogApp.BL.Dtos.Common;
using BlogApp.BL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController(IBlogService _blogService,ICommentService _commentService) : ControllerBase
    {
        /// <summary>
        /// Get all blogs with pagination
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        ///     GET /api/blogs?page=1&take=10
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] PageRequestDto request)
        {
            return Ok(await _blogService.GetAllAsync(request));
        }

        /// <summary>
        /// Get blogs with infinite scroll/cursor pagination
        /// </summary>
        /// <remarks>
        /// Example request:
        /// 
        ///     GET /api/blogs/feed?cursor=null&take=10
        /// </remarks>
        [HttpGet("feed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeed([FromQuery] CursorPagedRequestDto request)
        {
            return Ok(await _blogService.GetInfiniteScrollAsync(request));
        }

        /// <summary>
        /// Get a specific blog by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _blogService.GetByIdAsync(id));
        }

        /// <summary>
        /// Create a new blog post
        /// </summary>
        /// <remarks>
        /// Example request (form-data):
        /// 
        ///     POST /api/blogs
        ///     {
        ///         "title": "My First Blog Post",
        ///         "description": "This is a detailed description of my blog post.",
        ///         "coverImageUrl": "https://example.com/images/blog-cover.jpg",
        ///         "videoUrl": "https://example.com/videos/blog-video.mp4",
        ///         "categoryIds": [1, 2, 3]
        ///     }
        /// </remarks>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromForm]BlogCreateDto dto)
        {
            try 
            {
               await _blogService.CreateAsync(dto);
                return Ok();
            }
            catch (Exception ex) {return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Update an existing blog post
        /// </summary>
        /// <remarks>
        /// Example request (form-data):
        /// 
        ///     PUT /api/blogs/{id}
        ///     {
        ///         "title": "Updated Blog Post Title",
        ///         "description": "Updated description with new information.",
        ///         "categoryIds": [1, 2]
        ///     }
        /// </remarks>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromForm] BlogUpdateDto dto)
        {
            await _blogService.UpdateAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// Delete a blog post
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Delete(int id)
        {
            await _blogService.RemoveAsync(id);
            return Accepted();
        }

        /// <summary>
        /// Add a comment to a blog post
        /// </summary>
        /// <remarks>
        /// Example request (form-data):
        /// 
        ///     POST /api/blogs/comment/{id}
        ///     {
        ///         "text": "This is a great blog post! Very informative.",
        ///         "imageUrl": "https://example.com/images/comment-image.jpg",
        ///         "videoUrl": "https://example.com/videos/comment-video.mp4",
        ///         "parrentId": null
        ///     }
        /// </remarks>
        [Authorize]
        [HttpPost("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Comment(int id,[FromForm]CommentCreateDto dto )
        {
            await _commentService.CreateAsync(id ,dto);
            return Accepted();
        }

        /// <summary>
        /// Toggle like on a blog post
        /// </summary>
        [Authorize]
        [HttpPost("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Like(int id)
        {
            await _blogService.ToggleLikeAsync(id);
            return Ok();
        }
    }
}
