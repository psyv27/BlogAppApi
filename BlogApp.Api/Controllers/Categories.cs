using BlogApp.BL.Dtos.CategoryDtos;
using BlogApp.BL.Exceptions.Category;
using BlogApp.BL.Exceptions.Common;
using BlogApp.BL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Categories(ICategoryService _service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetAllAsync());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _service.GetByIdAsync(id));
            }
            catch (NegativIdException ex)
            {
                return BadRequest(new{ Message = ex.Message});
            }
            catch (CategoryNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm]CategoryCreateDto dto)
        {
           await _service.CreateAsync(dto);
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,[FromForm] CategoryUpdateDto dto)
        {
            await _service.UpdateAsync(id,dto);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.RemoveAsync(id);
            return NoContent();
        }
    }
}