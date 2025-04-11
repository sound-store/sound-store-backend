using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Commons
{
    /// <summary>
    /// Configure the primary key of the entity.
    /// </summary>
    public interface IEntity<T> 
    {
        [Key]
        T Id { get; set; }
    }
}
