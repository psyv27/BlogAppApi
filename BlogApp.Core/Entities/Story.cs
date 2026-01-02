using BlogApp.Core.Entities.Commons;
using System;

namespace BlogApp.Core.Entities
{
    public class Story : BaseEntitty
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
    }
}
