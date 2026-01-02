using BlogApp.Core.Entities.Commons;
using System;

namespace BlogApp.Core.Entities
{
    public class Message : BaseEntitty
    {
        public string SenderId { get; set; }
        public AppUser Sender { get; set; }
        public string ReceiverId { get; set; }
        public AppUser Receiver { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}
