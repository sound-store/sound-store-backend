using SoundStore.Core.Commons;

namespace SoundStore.Core.Entities
{
    public class SubCategory : AuditableEntity, IEntity<int>
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public ICollection<Product> Products { get; set; } = [];
    }
}
