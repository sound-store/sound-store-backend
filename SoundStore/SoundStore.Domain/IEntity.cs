using System.ComponentModel.DataAnnotations;

namespace SoundStore.Domain
{
    public interface IEntity<T>
    {
        [Key]
        T Id { get; set; }
    }
}
