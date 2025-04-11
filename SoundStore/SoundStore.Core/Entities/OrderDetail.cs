using SoundStore.Core.Commons;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundStore.Core.Entities
{
    public class OrderDetail : IEntity<long>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10)")]
        public decimal CurrentPrice { get; set; }

        public Product Product { get; set; } = null!;

        public Order Order { get; set; } = null!;
    }
}
