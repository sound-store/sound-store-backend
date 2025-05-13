namespace SoundStore.Domain
{
    public abstract class AuditableEntity
    {
        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
