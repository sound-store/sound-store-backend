using Microsoft.AspNetCore.Identity;
using SoundStore.Core.Enums;

namespace SoundStore.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public UserState Status { get; set; }

        public ICollection<Order> Orders { get; set; } = [];

        public ICollection<Transaction> Transactions { get; set; } = [];

        public ICollection<Rating> Ratings { get; set; } = [];
    }
}
