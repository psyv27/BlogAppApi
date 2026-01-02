using AutoMapper;
using BlogApp.BL.Dtos.StoryDtos;
using BlogApp.BL.Services.Interfaces;
using BlogApp.Core.Entities;
using BlogApp.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Implements
{
    public class StoryService(IStoryRepository _repo, IHttpContextAccessor _context, IFileService _fileService, IMapper _mapper) : IStoryService
    {
        readonly string? currentUserId = _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public async Task CreateStoryAsync(StoryCreateDto dto)
        {
            if (string.IsNullOrEmpty(currentUserId)) throw new UnauthorizedAccessException();

            var story = new Story
            {
                AppUserId = currentUserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                MediaUrl = await _fileService.UploadImageAsync(dto.MediaFile) // Assuming image for now
            };

            await _repo.CreateAsync(story);
            await _repo.SaveAsync();
        }

        public async Task<IEnumerable<StoryDto>> GetActiveStoriesAsync()
        {
            var now = DateTime.UtcNow;
            var stories = await _repo.Table
                .Include(s => s.AppUser)
                .Where(s => s.ExpiresAt > now)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StoryDto>>(stories);
        }
    }
}
