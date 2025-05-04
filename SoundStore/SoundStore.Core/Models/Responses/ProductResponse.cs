namespace SoundStore.Core.Models.Responses
{
    /// <summary>
    /// Represents the response model for a product.
    /// </summary>
    public class ProductResponse
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int StockQuantity { get; set; }

        public decimal Price { get; set; }

        public string? Type { get; set; }

        public string? Connectivity { get; set; }

        public string? SpecialFeatures { get; set; }

        public string? FrequencyResponse { get; set; }

        public string? Sensitivity { get; set; }

        public string? BatteryLife { get; set; }

        public string? AccessoriesIncluded { get; set; }

        public string? Warranty { get; set; }

        public int? SubCategoryId { get; set; }

        public string? SubCategoryName { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? Status { get; set; }

        public ICollection<ProductImage> Images { get; set; } = [];

        /// <summary>
        /// Average rating score of a product
        /// </summary>
        public decimal? OverallRatingScore { get; set; }

        public ICollection<RatingResponse> RatingResponses { get; set; } = [];
    }

    public class ProductImage
    {
        public string? ImageUrl { get; set; }
    }
}
