using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundStore.Core;
using SoundStore.Core.Commons;
using SoundStore.Core.Entities;
using SoundStore.Core.Enums;
using SoundStore.Core.Exceptions;
using SoundStore.Core.Models.Filters;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;
using SoundStore.Infrastructure.Helpers;

namespace SoundStore.Service
{
    public class ProductService(IUnitOfWork unitOfWork,
        ILogger<ProductService> logger,
        IImageService imageService) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly IImageService _imageService = imageService;

        public async Task<bool> AddProduct(ProductCreatedRequest request)
        {
            try
            {
                var productRepository = _unitOfWork.GetRepository<Product>();
                var productImageRepository = _unitOfWork.GetRepository<Image>();
                var isExisted = await productRepository.GetAll()
                    .AsNoTracking()
                    .AnyAsync(p => p.Name.ToLower() == request.Name.ToLower());
                if (isExisted) throw new DuplicatedException("Product has already existed!");

                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    StockQuantity = request.StockQuantity,
                    Price = request.Price,
                    Type = request.Type,
                    Connectivity = request.Connectivity,
                    SpecialFeatures = request.SpecialFeatures,
                    FrequencyResponse = request.FrequencyResponse,
                    Sensitivity = request.Sensitivity,
                    BatteryLife = request.BatteryLife,
                    AccessoriesIncluded = request.AccessoriesIncluded,
                    Warranty = request.Warranty,
                    SubCategoryId = request.SubCategoryId,
                    CreatedAt = DateTime.Now,
                    Status = ProductState.InStock   // default state of the product
                };
                productRepository.Add(product);
                foreach (var img in request.Images)
                {
                    var imgUrl = await _imageService.UploadImageAsync(img) ??
                        throw new Exception("Cannot upload the image!");
                    product.Images.Add(new Image
                    {
                        ProductId = product.Id,
                        Url = imgUrl,
                        CreatedAt = DateTime.Now
                    });
                }
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.StackTrace);
                throw;
            }
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
                .Include(p => p.Ratings)
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
                    }).ToList(),
                    OverallRatingScore = (decimal)p.Ratings.Average(r => r.RatingPoint),
                    RatingResponses = p.Ratings.Select(r => new RatingResponse
                    {
                        UserName = r.User.FirstName + " " + r.User.LastName,
                        RatingPoint = r.RatingPoint,
                        Comment = r.Comment,
                    }).ToList(),
                }).FirstOrDefaultAsync() ??
                    throw new KeyNotFoundException("No product found!");
            return product;
        }

        public async Task<bool> DeleteProduct(long productId)
        {
            try
            {
                var productRepository = _unitOfWork.GetRepository<Product>();
                var product = await productRepository.GetAll()
                    .AsNoTracking()
                    .Include(p => p.OrderDetails)
                    .FirstOrDefaultAsync(p => p.Id == productId)
                    ?? throw new KeyNotFoundException("Product does not exist!");
                if (product.OrderDetails.Any())
                    throw new Exception("Cannot delete this product because it is used in OrderDetails!");

                productRepository.Delete(product);
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<bool> UpdateProduct(long id, ProductUpdatedRequest request)
        {
            try
            {
                var productRepository = _unitOfWork.GetRepository<Product>();
                var product = await productRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id)
                    ?? throw new KeyNotFoundException("Product does not exist!");
                if (!Enum.TryParse<ProductState>(request.Status, true, out var productState))
                    throw new ArgumentException("Invalid product status!");
                // Update the existed product
                product.Name = request.Name;
                product.Description = request.Description;
                product.StockQuantity = request.StockQuantity;
                product.Price = request.Price;
                product.Type = request.Type;
                product.Connectivity = request.Connectivity;
                product.SpecialFeatures = request.SpecialFeatures;
                product.FrequencyResponse = request.FrequencyResponse;
                product.Sensitivity = request.Sensitivity;
                product.BatteryLife = request.BatteryLife;
                product.AccessoriesIncluded = request.AccessoriesIncluded;
                product.Warranty = request.Warranty;
                product.SubCategoryId = request.SubCategoryId;
                product.Status = productState;

                productRepository.Update(product);
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
