using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundStore.Core;
using SoundStore.Core.Commons;
using SoundStore.Core.Entities;
using SoundStore.Core.Enums;
using SoundStore.Core.Exceptions;
using SoundStore.Core.Models.Filters;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;
using SoundStore.Infrastructure.Helpers;

namespace SoundStore.Service
{
    public class ProductService(IUnitOfWork unitOfWork,
        ILogger<ProductService> logger) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ProductService> _logger = logger;

        public Task<bool> AddProduct()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.StackTrace);
                throw;
            }
        }

        public Task<bool> DeleteProduct()
        {
            throw new NotImplementedException();
        }

        public PaginatedList<ProductResponse> GetProducts(int pageNumber,
            int pageSize,
            ProductFilterParameters parameters,
            string sortByPrice)
        {
            var productRepository = _unitOfWork.GetRepository<Product>();
            if (parameters is not null)
            {
                var products = productRepository.GetAll()
                    .AsNoTracking()
                    .Include(p => p.SubCategory)
                    .Include(p => p.SubCategory!.Category)
                    .Include(p => p.Images)
                    .AsQueryable();

                // Filter by product's state
                if (!string.IsNullOrEmpty(parameters.Status) 
                    && Enum.TryParse<ProductState>(parameters.Status, out var productState))
                {
                    products = products.Where(p => p.Status == productState);
                }
                else
                {
                    products = products.Where(p => p.Status == ProductState.InStock);
                }
                // Filter by other parameters
                if (!string.IsNullOrEmpty(parameters.Name))
                {
                    products = products.Where(p => p.Name.Contains(parameters.Name, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(parameters.Name) && parameters.CategoryId.HasValue)
                {
                    products = products.Where(p => p.Name.Contains(parameters.Name, StringComparison.OrdinalIgnoreCase)
                        && p.SubCategory!.Category!.Id == parameters.CategoryId);
                }
                if (!string.IsNullOrEmpty(parameters.Name) && parameters.SubCategoryId.HasValue)
                {
                    products = products.Where(p => p.SubCategory!.Id == parameters.SubCategoryId);
                }

                if (!products.Any()) throw new NoDataRetrievalException("No products found.");
                var result = products.Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StockQuantity = p.StockQuantity,
                    Price = p.Price,
                    Type = p.Type,
                    Connectivity = p.Connectivity,
                    SpecialFeatures = p.SpecialFeatures,
                    FrequencyResponse = p.FrequencyResponse,
                    Sensitivity = p.Sensitivity,
                    BatteryLife = p.BatteryLife,
                    AccessoriesIncluded = p.AccessoriesIncluded,
                    Warranty = p.Warranty,
                    SubCategoryId = p.SubCategoryId,
                    SubCategoryName = p.SubCategory!.Name,
                    CategoryId = p.SubCategory.Category!.Id,
                    CategoryName = p.SubCategory.Category.Name,
                    Status = p.Status.ToString(),
                    Images = p.Images.Select(i => new ProductImage
                    {
                        ImageUrl = i.Url
                    }).ToList()
                });

                if (!string.IsNullOrEmpty(sortByPrice))
                {
                    if ("asc" == sortByPrice.ToLower().Trim())
                        result = result.OrderBy(p => p.Price);
                    else result = result.OrderByDescending(p => p.Price);
                }

                return PaginationHelper.CreatePaginatedList(result, pageNumber, pageSize);
            }
            else
            {
                var products = productRepository.GetAll()
                    .AsNoTracking()
                    .Include(p => p.SubCategory)
                    .Include(p => p.SubCategory!.Category)
                    .Include(p => p.Images)
                    .Where(p => p.Status == ProductState.InStock)
                    .Select(p => new ProductResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        StockQuantity = p.StockQuantity,
                        Price = p.Price,
                        Type = p.Type,
                        Connectivity = p.Connectivity,
                        SpecialFeatures = p.SpecialFeatures,
                        FrequencyResponse = p.FrequencyResponse,
                        Sensitivity = p.Sensitivity,
                        BatteryLife = p.BatteryLife,
                        AccessoriesIncluded = p.AccessoriesIncluded,
                        Warranty = p.Warranty,
                        SubCategoryId = p.SubCategoryId,
                        SubCategoryName = p.SubCategory!.Name,
                        CategoryId = p.SubCategory.Category!.Id,
                        CategoryName = p.SubCategory.Category.Name,
                        Status = p.Status.ToString(),
                        Images = p.Images.Select(i => new ProductImage
                        {
                            ImageUrl = i.Url
                        }).ToList()
                    }).AsQueryable();

                if (!products.Any())
                    throw new NoDataRetrievalException("No products found.");

                if (!string.IsNullOrEmpty(sortByPrice))
                {
                    if ("asc" == sortByPrice.ToLower().Trim())
                        products = products.OrderBy(p => p.Price);
                    else products = products.OrderByDescending(p => p.Price);
                }

                var response = PaginationHelper.CreatePaginatedList(products, pageNumber, pageSize);
                return response;
            }
        }

        public PaginatedList<ProductResponse> GetProductByCategory(int categoryId,
            int pageSize,
            int pageNumber)
        {
            var productRepository = _unitOfWork.GetRepository<Product>();
            var products = productRepository.GetAll()
                .AsNoTracking()
                .Include(p => p.SubCategory)
                .Include(p => p.SubCategory!.Category)
                .Include(p => p.Images)
                .Where(p => p.Status == ProductState.InStock
                            && p.SubCategory!.Category!.Id == categoryId)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StockQuantity = p.StockQuantity,
                    Price = p.Price,
                    Type = p.Type,
                    Connectivity = p.Connectivity,
                    SpecialFeatures = p.SpecialFeatures,
                    FrequencyResponse = p.FrequencyResponse,
                    Sensitivity = p.Sensitivity,
                    BatteryLife = p.BatteryLife,
                    AccessoriesIncluded = p.AccessoriesIncluded,
                    Warranty = p.Warranty,
                    SubCategoryId = p.SubCategoryId,
                    SubCategoryName = p.SubCategory!.Name,
                    CategoryId = p.SubCategory.Category!.Id,
                    CategoryName = p.SubCategory.Category.Name,
                    Status = p.Status.ToString(),
                    Images = p.Images.Select(i => new ProductImage
                    {
                        ImageUrl = i.Url
                    }).ToList()
                }).AsQueryable();

            if (!products.Any())
                throw new NoDataRetrievalException("No products found.");
            return PaginationHelper.CreatePaginatedList(products, pageNumber, pageSize);
        }
        
        public Task<bool> UpdateProduct()
        {
            throw new NotImplementedException();
        }

        public PaginatedList<ProductResponse> GetProductBySubCategory(int subCategoryId, 
            int pageNumber, 
            int pageSize)
        {
            var productRepository = _unitOfWork.GetRepository<Product>();
            var products = productRepository.GetAll()
                .AsNoTracking()
                .Include(p => p.SubCategory)
                .Include(p => p.SubCategory!.Category)
                .Include(p => p.Images)
                .Where(p => p.Status == ProductState.InStock
                            && p.SubCategoryId == subCategoryId)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StockQuantity = p.StockQuantity,
                    Price = p.Price,
                    Type = p.Type,
                    Connectivity = p.Connectivity,
                    SpecialFeatures = p.SpecialFeatures,
                    FrequencyResponse = p.FrequencyResponse,
                    Sensitivity = p.Sensitivity,
                    BatteryLife = p.BatteryLife,
                    AccessoriesIncluded = p.AccessoriesIncluded,
                    Warranty = p.Warranty,
                    SubCategoryId = p.SubCategoryId,
                    SubCategoryName = p.SubCategory!.Name,
                    CategoryId = p.SubCategory.Category!.Id,
                    CategoryName = p.SubCategory.Category.Name,
                    Status = p.Status.ToString(),
                    Images = p.Images.Select(i => new ProductImage
                    {
                        ImageUrl = i.Url
                    }).ToList()
                }).AsQueryable();

            if (!products.Any())
                throw new NoDataRetrievalException("No products found.");
            return PaginationHelper.CreatePaginatedList(products, pageNumber, pageSize);
        }

        public async Task<ProductResponse?> GetProduct(long id)
        {
            var productRepository = _unitOfWork.GetRepository<Product>();
            var product = await productRepository.GetAll()
                .AsNoTracking()
                .Include(p => p.SubCategory)
                .Include(p => p.SubCategory!.Category)
                .Include(p => p.Images)
                .Where(p => p.Id == id)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StockQuantity = p.StockQuantity,
                    Price = p.Price,
                    Type = p.Type,
                    Connectivity = p.Connectivity,
                    SpecialFeatures = p.SpecialFeatures,
                    FrequencyResponse = p.FrequencyResponse,
                    Sensitivity = p.Sensitivity,
                    BatteryLife = p.BatteryLife,
                    AccessoriesIncluded = p.AccessoriesIncluded,
                    Warranty = p.Warranty,
                    SubCategoryId = p.SubCategoryId,
                    SubCategoryName = p.SubCategory!.Name,
                    CategoryId = p.SubCategory.Category!.Id,
                    CategoryName = p.SubCategory.Category.Name,
                    Status = p.Status.ToString(),
                    Images = p.Images.Select(i => new ProductImage
                    {
                        ImageUrl = i.Url
                    }).ToList()
                }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("No product found!");
            
            return product;
        }
    }
}
