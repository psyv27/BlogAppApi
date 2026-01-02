using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.BL.Dtos.BlogDtos
{
    public record BlogUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? CoverImageFile { get; set; }
        public IFormFile? VideoFile { get; set; }
        public IEnumerable<int> CategoryIds { get; set; }
    }
}
