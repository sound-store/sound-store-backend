using Microsoft.EntityFrameworkCore;
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
    public class CategorySerivce(IUnitOfWork unitOfWork,
        ILogger<CategorySerivce> logger) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<CategorySerivce> _logger = logger;

        public async Task<bool> AddCategory(string name)
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

        public async Task<bool> UpdateCategory(int id, string name)
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
                
                var isExistedCategory = await category.GetAll()
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == categoryId);
                if (!isExistedCategory)
                    throw new KeyNotFoundException("Category is not found!");

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
