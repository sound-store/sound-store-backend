﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundStore.Core;
using SoundStore.Core.Commons;
using SoundStore.Core.Entities;
using SoundStore.Core.Exceptions;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;
using SoundStore.Infrastructure.Helpers;

namespace SoundStore.Service
{
    public class CategoryService(IUnitOfWork unitOfWork,
        ILogger<CategoryService> logger) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CategoryService> _logger = logger;

        public async Task<bool> AddCategory(string name, string description)
        {
            try
            {
                var categoryRepository = _unitOfWork.GetRepository<Category>();
                var isDuplicate = categoryRepository.GetAll()
                    .AsNoTracking()
                    .Any(c => c.Name.ToLower() == name.ToLower());
                if (isDuplicate)
                    throw new DuplicatedException("Category already exists!");

                var category = new Category
                {
                    Name = name,
                    Description = description,
                    CreatedAt = DateTime.Now,
                };
                categoryRepository.Add(category);
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<bool> AddSubCategory(int categoryId, string name)
        {
            try
            {
                var subCategoryRepository = _unitOfWork.GetRepository<SubCategory>();
                var categoryRepository = _unitOfWork.GetRepository<Category>();

                var isDuplicate = subCategoryRepository.GetAll()
                    .AsNoTracking()
                    .Any(c => c.Name.ToLower() == name.ToLower());
                if (isDuplicate)
                    throw new DuplicatedException("Sub category already exists!");

                var validCategory = await categoryRepository.GetAll()
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == categoryId);
                if (!validCategory)
                    throw new KeyNotFoundException("Category is not found!");

                var subCategory = new SubCategory
                {
                    Name = name,
                    CategoryId = categoryId,
                    CreatedAt = DateTime.Now,
                };
                subCategoryRepository.Add(subCategory);
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var categoryRepository = _unitOfWork.GetRepository<Category>();
                var category = await categoryRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (category is null)
                    throw new KeyNotFoundException("Category not found!");

                // INFO: Category and SubCategory are set delete set null
                categoryRepository.Delete(category);
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<bool> DeleteSubCategory(int id)
        {
            try
            {
                var subCategoryRepository = _unitOfWork.GetRepository<SubCategory>();
                var subCategory = await subCategoryRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (subCategory is null)
                    throw new KeyNotFoundException("Sub category is not found!");

                // INFO: SubCategory and Product are set delete set null
                subCategoryRepository.Delete(subCategory);
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public PaginatedList<CategoryResponse> GetCategories(string? name, int pageNumber, int pageSize)
        {
            var categoryRepository = _unitOfWork.GetRepository<Category>();
            if (string.IsNullOrEmpty(name))
            {
                var query = categoryRepository.GetAll()
                    .Include(c => c.SubCategories)
                    .AsNoTracking()
                    .Select(c => new CategoryResponse
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        SubCategories = c.SubCategories.Select(sc => new SubCategoryResponse
                        {
                            Id = sc.Id,
                            Name = sc.Name,
                            CategoryId = sc.CategoryId
                        }).ToList()
                    });
                if (!query.Any()) throw new NoDataRetrievalException("Category not found!");
                return PaginationHelper.CreatePaginatedList(query, pageNumber, pageSize);
            }
            else
            {
                var query = categoryRepository.GetAll()
                    .Include(c => c.SubCategories)
                    .AsNoTracking()
                    .Where(c => c.Name.ToLower().Contains(name.ToLower()))
                    .Select(c => new CategoryResponse
                    {
                        Id = c.Id,
                        Name = c.Name,
                        SubCategories = c.SubCategories.Select(sc => new SubCategoryResponse
                        {
                            Id = sc.Id,
                            Name = sc.Name,
                            CategoryId = sc.CategoryId
                        }).ToList()
                    });
                if (!query.Any()) throw new NoDataRetrievalException("Category not found!");
                return PaginationHelper.CreatePaginatedList(query, pageNumber, pageSize);
            }
        }

        public async Task<List<CategoryResponse>> GetCategories()
        {
            var categoryRepository = _unitOfWork.GetRepository<Category>();
            var categories = await categoryRepository.GetAll()
                .AsNoTracking()
                .ToListAsync();
            if (!categories.Any()) throw new NoDataRetrievalException("No data of categories!");

            var response = new List<CategoryResponse>();
            foreach (var category in categories)
            {
                var t = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                };
                response.Add(t);
            }
            return response;
        }

        public async Task<CategoryResponse?> GetCategory(int id)
        {
            var categoryRepository = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepository.GetAll()
                .Include(c => c.SubCategories)
                .AsNoTracking()
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    SubCategories = c.SubCategories.Select(sc => new SubCategoryResponse
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        CategoryId = sc.CategoryId
                    }).ToList()
                })
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category is null)
            {
                throw new KeyNotFoundException("Category does not exist!");
            }
            return category;
        }

        public async Task<bool> UpdateCategory(int id, string name, string description)
        {
            try
            {
                var categoryRepository = _unitOfWork.GetRepository<Category>();
                var category = await categoryRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (category is null)
                    throw new KeyNotFoundException("Category not found!");

                category.Name = name;
                category.Description = description;
                category.UpdatedAt = DateTime.Now;

                categoryRepository.Update(category);
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<bool> UpdateSubCategory(int id, string name, int? categoryId)
        {
            try
            {
                var category = _unitOfWork.GetRepository<Category>();
                var subCategoryRepository = _unitOfWork.GetRepository<SubCategory>();

                var subCategory = await subCategoryRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (subCategory is null)
                    throw new KeyNotFoundException("Sub category is not found!");

                var existedCategory = await category.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == categoryId)
                    ?? throw new KeyNotFoundException("Category does not exist!");

                if (categoryId.HasValue)
                {
                    subCategory.Name = name;
                    subCategory.CategoryId = categoryId;
                    subCategory.UpdatedAt = DateTime.Now;
                }
                else
                {
                    subCategory.Name = name;
                    subCategory.UpdatedAt = DateTime.Now;
                }

                subCategoryRepository.Update(subCategory);
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }
    }
}
