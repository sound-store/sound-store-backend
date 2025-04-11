using SoundStore.Core.Commons;

namespace SoundStore.Core.Entities
{
    public class Image : IEntity<long>
    {
        public long Id { get; set; }

        public long? ProductId { get; set; }

        public string? Url { get; set; }

        public DateTime? CreatedAt { get ; set ; }

        public DateTime? UpdatedAt { get; set; }

        public Product? Product { get; set; }
    }
}
