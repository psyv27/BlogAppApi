using AutoMapper;
using BlogApp.BL.Dtos.MessageDtos;
using BlogApp.BL.Services.Interfaces;
using BlogApp.Core.Entities;
using BlogApp.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Implements
{
    public class MessageService(IMessageRepository _repo, IHttpContextAccessor _context, UserManager<AppUser> _userManager, IMapper _mapper) : IMessageService
    {
        readonly string? currentUserId = _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public async Task<IEnumerable<MessageDto>> GetConversationAsync(string otherUserId)
        {
            if (string.IsNullOrEmpty(currentUserId)) throw new UnauthorizedAccessException();

            var messages = await _repo.Table
                .Include(m => m.Sender)
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task SendMessageAsync(MessageCreateDto dto)
        {
            if (string.IsNullOrEmpty(currentUserId)) throw new UnauthorizedAccessException();
            
            var receiver = await _userManager.FindByIdAsync(dto.ReceiverId);
            if (receiver == null) throw new Exception("User not found");

            var message = new Message
            {
                SenderId = currentUserId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _repo.CreateAsync(message);
            await _repo.SaveAsync();
        }
    }
}
