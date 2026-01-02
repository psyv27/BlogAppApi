using AutoMapper;
using BlogApp.BL.Dtos.CategoryDtos;
using BlogApp.BL.Exceptions.Category;
using BlogApp.BL.Exceptions.Common;

using BlogApp.Core.Entities;
using BlogApp.DAL.Repositories.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Interfaces
{
    public class CategoryService(ICategoryRepository _repo,IMapper _mapper) : ICategoryService
    {
     

        public async Task CreateAsync(CategoryCreateDto dto)
        {
     
        

            // Map DTO to entity
            Category category = _mapper.Map<Category>(dto);
            category.LogoUrl = "default_logo_url"; // Set default logo URL or handle this differently

            // Save to repository
            await _repo.CreateAsync(category);
            await _repo.SaveAsync();
        }
    
    ////public async Task<IEnumerable<Category>> GetAllAsync()
    ////    =>await _repo.GetAll().ToListAsync();

    //public async Task<Category> GetByIdAsync(int id)
    //{
    //    if (id <= 0) throw new NegativIdExeption();
    //    var entity = await _repo.FindByIdAsync(id);
    //    if (entity == null) throw new CategoryNotFoundExeption();
    //    return entity;
    //}

    public async Task RemoveAsync(int id)
        {
           var entity = await _gategoryAsync(id);
            _repo.Delete(entity);
            _repo.SaveAsync();
        }

        public async Task UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var entity =await _gategoryAsync(id);
            _mapper.Map(dto, entity);
            //_mapper.Map<Category>(dto); eyni seydi
           await  _repo.SaveAsync();
        }

        public async Task<IEnumerable<CategoryListItemDto>> GetAllAsync()
        {
          //return await _repo.GetAll().Select(c=> new CategoryListItemDto
          //{ Id=c.Id, Name=c.Name, LogoUrl=c.LogoUrl, IsDeleted=c.IsDeleted }).ToListAsync();

            return _mapper.Map<IEnumerable<CategoryListItemDto>>(_repo.GetAll());

        }

       public async Task<CategoryDetailDto> GetByIdAsync(int id)
        {
            var entity =await _gategoryAsync(id);
            return _mapper.Map<CategoryDetailDto>(entity);
        }

       async Task<Category> _gategoryAsync(int id)
        {

            if (id <= 0) throw new NegativIdException();
            var entity = await _repo.FindByIdAsync(id);
            if (entity == null) throw new NotFoundException<Category>();
            return entity;
        }
    }
}
