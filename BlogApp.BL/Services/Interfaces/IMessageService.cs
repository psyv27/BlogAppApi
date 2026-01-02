using BlogApp.BL.Dtos.MessageDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Interfaces
{
    public interface IMessageService
    {
        Task SendMessageAsync(MessageCreateDto dto);
        Task<IEnumerable<MessageDto>> GetConversationAsync(string userId);
    }
}
