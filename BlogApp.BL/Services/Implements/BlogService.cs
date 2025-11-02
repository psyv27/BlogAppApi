using AutoMapper;
using BlogApp.BL.Dtos.BlogDtos;
using BlogApp.BL.Dtos.CategoryDtos;
using BlogApp.BL.Exceptions.Category;
using BlogApp.BL.Exceptions.Common;
using BlogApp.BL.Exceptions.User;

using BlogApp.BL.Services.Interfaces;
using BlogApp.Core.Entities;
using BlogApp.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Implements
{
    public class BlogService(IBlogRepository _repo,IMapper _mapper ,IHttpContextAccessor _context,ICategoryRepository _categoryRepo,UserManager<AppUser> _userManager, IFileService _fileService) : IBlogService
    {
        readonly string? userId = _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public async Task CreateAsync(BlogCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException();
            if (!await _userManager.Users.AnyAsync(u => u.Id == userId)) throw new UserNotFoundException();
            List<BlogCategory> blogCats = new();
            foreach (var id in dto.CategoryIds)
            {
                //var cat = await _categoryRepo.FindByIdAsync(id);
                //if (cat == null) throw new CategoryNotFoundException();

                if (!await _categoryRepo.IsExistAsync(c => c.Id == id && !c.IsDeleted)) throw new CategoryNotFoundException();
                blogCats.Add(new BlogCategory { CategoryId = id });
            }
            Blog blog = _mapper.Map<Blog>(dto);
            blog.AppUserId = userId;
            blog.BlogCategories = blogCats;
            
            // Handle file uploads
            if (dto.CoverImageFile != null)
            {
                blog.CoverImageUrl = await _fileService.UploadImageAsync(dto.CoverImageFile);
            }
            
            if (dto.VideoFile != null)
            {
                blog.VideoUrl = await _fileService.UploadVideoAsync(dto.VideoFile);
            }
            
            await _repo.CreateAsync(blog);
            await _repo.SaveAsync();
        }

        public async Task<IEnumerable<BlogListItemDto>> GetAllAsync()
        {
            //
            var entity = _repo.GetAll("AppUser", "BlogCategories", "BlogCategories.Category","Comments" ,"Comments.Children","Comments.AppUser");
            return _mapper.Map<IEnumerable<BlogListItemDto>>(entity);
        }

      

        public async Task<BlogDetailDto> GetByIdAsync(int id)
        {
           
            var entity = await _repo.GetAll(
                    "AppUser",                  
                    "BlogCategories.Category",  
                    "Comments",                 
                    "Comments.AppUser",         
                    "Comments.Children"         
                )
            
                .SingleOrDefaultAsync(b => b.Id == id);
           

            if (entity == null) throw new NotFoundException<Blog>();

            entity.ViewerCount++;
            await _repo.SaveAsync();

           
            return _mapper.Map<BlogDetailDto>(entity);
        }

        public async Task RemoveAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException();
            if (!await _userManager.Users.AnyAsync(u => u.Id == userId)) throw new UserNotFoundException();
            var entity = await _repo.FindByIdAsync(id);
            if (entity == null) throw new NotFoundException<Blog>();
            if (entity.AppUserId != userId) throw new Exception("icaze yoxdur ");
            _repo.SoftDelete(entity);
            await _repo.SaveAsync();

        }

        public Task UpdateAsync(int id, BlogUpdateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
