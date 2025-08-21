using AutoMapper;
using BlogApp.BL.Dtos.CategoryDtos;
using BlogApp.BL.Dtos.CommentDtos;
using BlogApp.BL.Exceptions.Common;
using BlogApp.BL.Exceptions.User;
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
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Implements
{
    public class CommentService(ICommentRepository _repo, IMapper _mapper, IHttpContextAccessor _context, IBlogRepository _blogRepo, UserManager<AppUser> _userManager, IFileService _fileService) : ICommentService
    {
        readonly string? userId = _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
        public async Task CreateAsync(int id, CommentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException();
            if (!await _userManager.Users.AnyAsync(u => u.Id == userId)) throw new UserNotFoundException();
            if (id < 0) throw new NegativIdException();
            if (!await _blogRepo.IsExistAsync(b => b.Id == id)) throw new NotFoundException<Blog>();
            var comment = _mapper.Map<Comment>(dto);
            comment.AppUserId = userId;
            comment.BlogId = id;
            
            // Handle file uploads
            if (dto.ImageFile != null)
            {
                comment.ImageUrl = await _fileService.UploadImageAsync(dto.ImageFile);
            }
            
            if (dto.VideoFile != null)
            {
                comment.VideoUrl = await _fileService.UploadVideoAsync(dto.VideoFile);
            }
            
            comment.CreatedDate = DateTime.UtcNow;
            await _repo.CreateAsync(comment);
            await _repo.SaveAsync();

        }

        public Task<IEnumerable<CommentListItemDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CommentDetailDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, CategoryUpdateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
