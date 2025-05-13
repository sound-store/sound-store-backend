using System.ComponentModel.DataAnnotations.Schema;

namespace SoundStore.Domain.Entities
{
    public class Product : AuditableEntity, IEntity<long>
    {
        public long Id { get; set; }

        /// <summary>
        /// Product's code. Format: "PO-xxxx"
        /// </summary>
        public string Code { get; set; } = null!;

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

    }
}
