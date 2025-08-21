using BlogApp.BL.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.BL.Dtos.BlogDtos
{
    public record BlogDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int ViewerCount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedTime { get; set; }
        public AuthorDto AppUser { get; set; }
        public IEnumerable<BlogCategoryDto> BlogCategories { get; set; }
    }
}
