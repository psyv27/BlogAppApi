using BlogApp.BL.Dtos.BlogDtos;
using BlogApp.BL.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogListItemDto>> GetAllAsync(PageRequestDto request);
        Task<IEnumerable<BlogListItemDto>> GetInfiniteScrollAsync(CursorPagedRequestDto request);
        Task CreateAsync(BlogCreateDto dto);
        Task<BlogDetailDto> GetByIdAsync(int id);
        Task RemoveAsync(int id);
        Task UpdateAsync(int id, BlogUpdateDto dto);
        Task ToggleLikeAsync(int id);
    }
}
