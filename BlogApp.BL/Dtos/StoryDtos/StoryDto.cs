using System;

namespace BlogApp.BL.Dtos.StoryDtos
{
    public record StoryDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public string UserName { get; set; }
        public string UserImageUrl { get; set; }
        public string MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
