using Microsoft.AspNetCore.Http;

namespace BlogApp.BL.Dtos.StoryDtos
{
    public record StoryCreateDto
    {
        public IFormFile MediaFile { get; set; }
    }
}
