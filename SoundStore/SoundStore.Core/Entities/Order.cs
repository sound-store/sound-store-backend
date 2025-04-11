using SoundStore.Core.Commons;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundStore.Core.Entities
{
    public class Order : IEntity<long>
    {
        /// <summary>
        /// 6 digit number
        /// </summary>
        public long Id { get; set; }

        [Column(TypeName = "decimal(10)")]
        public decimal Total { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string UserId { get; set; } = null!;

        public AppUser User { get; set; } = null!;

        public ICollection<OrderDetail> OrderDetails { get; set; } = [];

        public Transaction Transaction { get; set; } = null!;
    }
}
