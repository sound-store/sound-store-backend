using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    // TODO: Load the existed product's image, add the property for uploading new image
    public class ProductUpdatedRequest
    {
        [Required(ErrorMessage = "Product name is required!",
            AllowEmptyStrings = false)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Product description is required!",
            AllowEmptyStrings = false)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Stock quantity is required!")]
        [Range(1, 100, ErrorMessage = "Quantity must be in 1 and 100!")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Price is required!")]
        [Range(1000, 100000000,
            ErrorMessage = "Price must be in 1000 and 1000.000.000 VND")]
        public decimal Price { get; set; }

        public string? Type { get; set; }

        public string? Connectivity { get; set; }

        public string? SpecialFeatures { get; set; }

        public string? FrequencyResponse { get; set; }

        public string? Sensitivity { get; set; }

        public string? BatteryLife { get; set; }

        public string? AccessoriesIncluded { get; set; }

        public string? Warranty { get; set; }

        public int SubCategoryId { get; set; }

        public string Status { get; set; } = null!;
    }
}
