using BlogApp.Core.Entities;
using BlogApp.DAL.Repositories.Interfaces;
using BlogApp.DAL.Contexts;

namespace BlogApp.DAL.Repositories.Implements
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
