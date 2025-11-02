using AutoMapper;
using BlogApp.BL.Dtos.CategoryDtos;
using BlogApp.BL.Exceptions.Category;
using BlogApp.BL.Exceptions.Common;
using BlogApp.Core.Entities;
using BlogApp.DAL.Repositories.Interfaces;
using BlogApp.BL.Services.Interfaces; // Убедитесь, что IFileService здесь
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Implements // Убедитесь, что Implements правильное пространство имен
{
    // 1. Добавьте IFileService в конструктор
    public class CategoryService(
        ICategoryRepository _repo,
        IMapper _mapper,
        IHttpContextAccessor _accessor,
        IFileService _fileService) : ICategoryService
    {
        public async Task CreateAsync(CategoryCreateDto dto)
        {
            // Проверка на null
            if (dto.Logo == null)
                throw new ValidationException("Logo file is required.");

            // Маппинг DTO в сущность
            Category category = _mapper.Map<Category>(dto);

            // 2. Исправлено: Загружаем файл и получаем URL
            category.LogoUrl = await _fileService.UploadImageAsync(dto.Logo);

            // Сохраняем в репозиторий
            await _repo.CreateAsync(category);
            await _repo.SaveAsync();
        }

        public async Task RemoveAsync(int id)
        {
            var entity = await _getCategoryAsync(id); // 5. Исправлен вызов
            _repo.Delete(entity);

            // 3. Исправлено: Добавлен await
            await _repo.SaveAsync();
        }

        public async Task UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var entity = await _getCategoryAsync(id); // 5. Исправлен вызов

            // Обновляем поля, которые маппятся (например, Name)
            _mapper.Map(dto, entity);

            // 4. Исправлено: Добавлена логика обновления файла
            if (dto.Logo != null)
            {
                // Удаляем старый файл, если он есть
                if (!string.IsNullOrEmpty(entity.LogoUrl))
                {
                    await _fileService.DeleteFileAsync(entity.LogoUrl);
                }

                // Загружаем новый файл
                entity.LogoUrl = await _fileService.UploadImageAsync(dto.Logo);
            }

            await _repo.SaveAsync();
        }

        public async Task<IEnumerable<CategoryListItemDto>> GetAllAsync()
        {
            // Этот метод уже был исправлен в прошлый раз и выглядит корректно
            var request = _accessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return await _repo.GetAll()
                .Select(c => new CategoryListItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    LogoUrl = string.IsNullOrEmpty(c.LogoUrl)
                        ? null
                        : $"{baseUrl}/{c.LogoUrl.Replace("\\", "/")}",
                    IsDeleted = c.IsDeleted
                }).ToListAsync();
        }

        public async Task<CategoryDetailDto> GetByIdAsync(int id)
        {
            var entity = await _getCategoryAsync(id); // 5. Исправлен вызов

            // Здесь тоже нужно сформировать полный URL
            var dto = _mapper.Map<CategoryDetailDto>(entity);

            if (!string.IsNullOrEmpty(dto.LogoUrl))
            {
                var request = _accessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                dto.LogoUrl = $"{baseUrl}/{dto.LogoUrl.Replace("\\", "/")}";
            }

            return dto;
        }

        // 5. Исправлено: Опечатка в названии метода
        async Task<Category> _getCategoryAsync(int id)
        {
            if (id <= 0) throw new NegativIdException();
            var entity = await _repo.FindByIdAsync(id);
            if (entity == null) throw new NotFoundException<Category>();
            return entity;
        }
    }
}