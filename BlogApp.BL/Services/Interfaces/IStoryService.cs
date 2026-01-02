using BlogApp.BL.Dtos.StoryDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Interfaces
{
    public interface IStoryService
    {
        Task CreateStoryAsync(StoryCreateDto dto);
        Task<IEnumerable<StoryDto>> GetActiveStoriesAsync();
    }
}
