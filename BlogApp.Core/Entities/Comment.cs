using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Core.Entities
{
    public class Comment : BaseEntitty
    {
        public string Text { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int? ParrentId { get; set; }
        public Comment? Parrent { get; set; }
        public IEnumerable<Comment>? Children
        {
            get; set;
        }
    }
}
