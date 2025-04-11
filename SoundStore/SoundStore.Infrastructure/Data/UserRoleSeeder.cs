using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoundStore.Core.Constants;

namespace SoundStore.Infrastructure.Data
{
    public static class UserRoleSeeder
    {
        public static void SeedUserRoles(this ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = UserRoles.Admin,
                    NormalizedName = UserRoles.Admin.ToUpper(),
                    ConcurrencyStamp = "abc"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = UserRoles.Customer,
                    NormalizedName = UserRoles.Customer.ToUpper(),
                    ConcurrencyStamp = "def"
                }
            );
        }
    }
}
