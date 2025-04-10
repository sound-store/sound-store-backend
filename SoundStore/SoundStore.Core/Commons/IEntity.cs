namespace SoundStore.Core.Commons
{
    /// <summary>
    /// Configure the primary key of the entity.
    /// </summary>
    public interface IEntity<T> 
    {
        T Id { get; set; }
    }
}
