using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundStore.Core;
using SoundStore.Core.Commons;
using SoundStore.Core.Entities;
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
            // TODO: Đối với user và Customer thì k lấy field status của product
            if (parameters is not null)
            {
                var products = productRepository.GetAll()
                    .AsNoTracking()
                    .Include(p => p.SubCategory)
                    .Include(p => p.SubCategory.Category)
                    .Include(p => p.Images);

                var filteredQuery = products.ApplyFilters(parameters);
                if (!filteredQuery.Any())
                    throw new NoDataRetrievalException("No products found.");
                var result = filteredQuery.Select(p => new ProductResponse
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
                    SubCategoryName = p.SubCategory.Name,
                    CategoryId = p.SubCategory.Category.Id,
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
                    .Include(p => p.SubCategory.Category)
                    .Include(p => p.Images)
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
                        SubCategoryName = p.SubCategory.Name,
                        CategoryId = p.SubCategory.Category.Id,
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
        
        public Task<bool> UpdateProduct()
        {
            throw new NotImplementedException();
        }
    }
}
