using SoundStore.Core.Commons;
using SoundStore.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundStore.Core.Entities
{
    /// <summary>
    /// The product are headphones and speakers with common properties
    /// </summary>
    public class Product : AuditableEntity, IEntity<long>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int StockQuantity { get; set; }

        [Column(TypeName = "decimal(10)")]
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

        public ProductState Status { get; set; }

        public SubCategory? SubCategory { get; set; }

        public ICollection<Image> Images { get; set; } = [];

        public ICollection<OrderDetail> OrderDetails { get; set; } = [];

        public ICollection<Rating> Ratings { get; set; } = [];
    }
}
