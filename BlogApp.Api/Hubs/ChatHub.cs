using BlogApp.BL.Dtos.MessageDtos;
using BlogApp.BL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BlogApp.API.Hubs
{
    [Authorize]
    public class ChatHub(IMessageService _messageService) : Hub
    {
        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.UserIdentifier;
            
            // Save to DB
            await _messageService.SendMessageAsync(new MessageCreateDto 
            { 
                ReceiverId = receiverId, 
                Content = content 
            });

            // Send to receiver (assuming userId is used as group or user identifier)
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, content);
        }
    }
}
