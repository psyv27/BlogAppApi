using BlogApp.Core.Entities.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Core.Entities
{
    public class BlogLike : BaseEntitty
    {
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
