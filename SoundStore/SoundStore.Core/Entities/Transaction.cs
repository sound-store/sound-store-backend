using SoundStore.Core.Commons;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SoundStore.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Transaction : IEntity<long>
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public string UserId { get; set; } = null!;

        [Column(TypeName = "decimal(10)")]
        public decimal Amount { get; set; }

        public DateTime? CreatedAt { get; set; }

        public Order Order { get; set; } = null!;

        public AppUser User { get; set; } = null!;
    }
}
