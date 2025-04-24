using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoundStore.Core.Validations;
using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    public class ProductCreatedRequest
    {
        [Required(ErrorMessage = "Product name is required!",
            AllowEmptyStrings = false)]
        [BindProperty(Name = "name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Product description is required!",
            AllowEmptyStrings = false)]
        [BindProperty(Name = "description")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Stock quantity is required!")]
        [Range(1, 100, ErrorMessage = "Quantity must be in 1 and 100!")]
        [BindProperty(Name = "stockQuantity")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Price is required!")]
        [Range(1000, 100000000,
            ErrorMessage = "Price must be in 1000 and 1000.000.000 VND")]
        [BindProperty(Name = "price")]
        public decimal Price { get; set; }

        [BindProperty(Name = "type")]
        public string? Type { get; set; }

        [BindProperty(Name = "connectivity")]
        public string? Connectivity { get; set; }

        [BindProperty(Name = "specialFeatures")]
        public string? SpecialFeatures { get; set; }

        [BindProperty(Name = "frequencyResponse")]
        public string? FrequencyResponse { get; set; }

        [BindProperty(Name = "sensitivity")]
        public string? Sensitivity { get; set; }

        [BindProperty(Name = "batteryLife")]
        public string? BatteryLife { get; set; }

        [BindProperty(Name = "accessoriesIncluded")]
        public string? AccessoriesIncluded { get; set; }

        [BindProperty(Name = "warranty")]
        public string? Warranty { get; set; }

        [Required(ErrorMessage = "Subcategory is required!")]
        [BindProperty(Name = "subCategoryId")]
        public int SubCategoryId { get; set; }

        [AllowedImageFiles]
        [BindProperty(Name = "images")]
        public List<IFormFile> Images { get; set; } = [];
    }
}
