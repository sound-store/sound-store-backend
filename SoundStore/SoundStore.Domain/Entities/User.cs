using SoundStore.Domain.Enums;

namespace SoundStore.Domain.Entities
{
    public class User : AuditableEntity, IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public UserState Status { get; set; }

        public string Role { get; set; } = null!;
    }
}
