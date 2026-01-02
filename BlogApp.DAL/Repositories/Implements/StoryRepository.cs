using BlogApp.Core.Entities;
using BlogApp.DAL.Repositories.Interfaces;
using BlogApp.DAL.Contexts;

namespace BlogApp.DAL.Repositories.Implements
{
    public class StoryRepository : Repository<Story>, IStoryRepository
    {
        public StoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
