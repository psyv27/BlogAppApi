using AutoMapper;
using BlogApp.BL.Dtos.BlogDtos;
using BlogApp.BL.Dtos.CategoryDtos;
using BlogApp.BL.Dtos.Common;
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
    public class BlogService(IBlogRepository _repo,IMapper _mapper ,IHttpContextAccessor _context,ICategoryRepository _categoryRepo,UserManager<AppUser> _userManager, IFileService _fileService, IBlogLikeRepository _likeRepo) : IBlogService
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

        public async Task<IEnumerable<BlogListItemDto>> GetAllAsync(PageRequestDto request)
        {
<<<<<<< HEAD
            var query = _repo.Table
                .Include(b => b.AppUser)
                .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
                .Include(b => b.Comments).ThenInclude(c => c.Children)
                .Include(b => b.Comments).ThenInclude(c => c.AppUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(b => b.Title.Contains(request.Search) || b.Description.Contains(request.Search));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(b => b.BlogCategories.Any(bc => bc.CategoryId == request.CategoryId));
            }

            query = query.Skip((request.Page - 1) * request.Size).Take(request.Size);

            var entities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<BlogListItemDto>>(entities);
        }

        public async Task<IEnumerable<BlogListItemDto>> GetInfiniteScrollAsync(CursorPagedRequestDto request)
        {
            var query = _repo.Table
                .Include(b => b.AppUser)
                .Include(b => b.BlogCategories).ThenInclude(bc => bc.Category)
                .Include(b => b.Comments).ThenInclude(c => c.Children)
                .Include(b => b.Comments).ThenInclude(c => c.AppUser)
                .AsQueryable();

            if (request.Cursor.HasValue)
            {
                query = query.Where(b => b.CreatedTime < request.Cursor.Value);
            }

            query = query.OrderByDescending(b => b.CreatedTime)
                         .Take(request.Size);

            var entities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<BlogListItemDto>>(entities);
=======
            //var dto = new List<BlogListItemDto>();
            //var entity = _repo.GetAll("AppUser", "BlogCategories", "BlogCategories.Category");
            //List<Category> categories = new();

            //foreach (var item in entity)
            //{
            //    categories.Clear();
            //    foreach (var category in item.BlogCategories)
            //    {
            //        categories.Add(category.Category);
            //    }
            //    var dtoItem = _mapper.Map<BlogListItemDto>(item);
            //    dtoItem.Categories = _mapper.Map<IEnumerable<CategoryListItemDto>>(categories);

            //    dto.Add(dtoItem);
            //}
            //return dto;
            var entity = _repo.GetAll("AppUser", "BlogCategories", "BlogCategories.Category","Comments" ,"Comments.Children","Comments.AppUser");
            return _mapper.Map<IEnumerable<BlogListItemDto>>(entity);
>>>>>>> parent of 77b7f0a (update)
        }

        public async Task<BlogDetailDto> GetByIdAsync(int id)
        {
           var entity = await _repo.FindByIdAsync(id);
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
            
            var user = _context.HttpContext?.User;
            if (entity.AppUserId != userId && (user == null || !user.IsInRole("Admin"))) 
                throw new Exception("icaze yoxdur ");
            
            _repo.SoftDelete(entity);
            await _repo.SaveAsync();

        }

        public async Task UpdateAsync(int id, BlogUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException();

            var entity = await _repo.Table
                .Include(b => b.BlogCategories)
                .SingleOrDefaultAsync(b => b.Id == id);

            if (entity == null) throw new NotFoundException<Blog>();

            var user = _context.HttpContext?.User;
            if (entity.AppUserId != userId && (user == null || !user.IsInRole("Admin")))
                throw new Exception("icaze yoxdur ");

            entity.Title = dto.Title;
            entity.Description = dto.Description;

            if (dto.CoverImageFile != null)
            {
                entity.CoverImageUrl = await _fileService.UploadImageAsync(dto.CoverImageFile);
            }

            if (dto.VideoFile != null)
            {
                entity.VideoUrl = await _fileService.UploadVideoAsync(dto.VideoFile);
            }

            if (dto.CategoryIds != null)
            {
                // Remove categories that are not in the new list
                var toRemove = entity.BlogCategories.Where(bc => !dto.CategoryIds.Contains(bc.CategoryId)).ToList();
                foreach (var item in toRemove)
                {
                    entity.BlogCategories.Remove(item);
                }

                // Add new categories
                var existingCategoryIds = entity.BlogCategories.Select(bc => bc.CategoryId).ToList();
                var toAdd = dto.CategoryIds.Where(id => !existingCategoryIds.Contains(id)).ToList();

                foreach (var catId in toAdd)
                {
                    if (!await _categoryRepo.IsExistAsync(c => c.Id == catId && !c.IsDeleted)) throw new CategoryNotFoundException();
                    entity.BlogCategories.Add(new BlogCategory { CategoryId = catId });
                }
            }

            await _repo.SaveAsync();
        }

        public async Task ToggleLikeAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException();
            
            var blog = await _repo.FindByIdAsync(id);
            if (blog == null) throw new NotFoundException<Blog>();

            var existingLike = await _likeRepo.GetSingleAsync(l => l.BlogId == id && l.AppUserId == userId);
            
            if (existingLike != null)
            {
                _likeRepo.Delete(existingLike);
            }
            else
            {
                await _likeRepo.CreateAsync(new BlogLike { BlogId = id, AppUserId = userId });
            }
            
            await _likeRepo.SaveAsync();
        }
    }
}
