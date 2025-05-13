using System.ComponentModel.DataAnnotations.Schema;

namespace SoundStore.Domain.Entities
{
    public class Category : AuditableEntity, IEntity<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
